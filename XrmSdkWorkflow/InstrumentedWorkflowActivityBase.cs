using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLLC.Telemetry;
using CCLLC.Telemetry.Client;
using CCLLC.Telemetry.Context;
using CCLLC.Telemetry.EventLogger;
using CCLLC.Telemetry.Serializer;
using CCLLC.Telemetry.Sink;


namespace CCLLC.Xrm.Sdk.Workflow
{
    public abstract partial class InstrumentedWorkflowActivityBase : WorkflowActivityBase, ISupportWorkflowActivityInstrumentation
    {       

        /// <summary>
        /// Provides a <see cref="ITelemetrySink"/> to recieve and process various 
        /// <see cref="ITelemetry"/> items generated during the execution of the 
        /// WorkflowActivity.
        /// </summary>
        public virtual ITelemetrySink TelemetrySink { get; private set; }
       
        public InstrumentedWorkflowActivityBase()
            : base()
        {
            this.TelemetrySink = Container.Resolve<ITelemetrySink>();
        }

        /// <summary>
        /// Registers all dependencies used by the WorkflowActivity. 
        /// </summary>
        public override void RegisterContainerServices()
        {
            base.RegisterContainerServices();

            //Telemetry issue event logger
            Container.RegisterAsSingleInstance<IEventLogger, InertEventLogger>();

            //setup the objects needed to create/capture telemetry items.
            Container.RegisterAsSingleInstance<ITelemetryFactory, TelemetryFactory>();  //ITelemetryFactory is used to create new telemetry items.
            Container.RegisterAsSingleInstance<ITelemetryClientFactory, TelemetryClientFactory>(); //ITelemetryClientFactory is used to create and configure a telemetry client.
            Container.RegisterAsSingleInstance<IXrmTelemetryPropertyManager, Telemetry.ExecutionContextPropertyManager>(); //Plugin property manager.
            Container.Register<ITelemetryContext, TelemetryContext>(); //ITelemetryContext is a dependency for telemetry creation.
            Container.Register<ITelemetryInitializerChain, TelemetryInitializerChain>(); //ITelemetryInitializerChain is a dependency for building a telemetry client.

            //setup the objects needed to buffer and send telemetry to Application Insights.
            Container.Register<ITelemetrySink, TelemetrySink>(); //ITelemetrySink receives telemetry from one or more telemetry clients, processes it, buffers it, and transmits it.
            Container.Register<ITelemetryProcessChain, TelemetryProcessChain>(); //ITelemetryProcessChain holds 0 or more processors that can modify the telemetry prior to transmission.
            Container.Register<ITelemetryChannel, SyncMemoryChannel>(); //ITelemetryChannel provides the buffering and transmission. There is a sync and an asynch channel.
            Container.Register<ITelemetryBuffer, TelemetryBuffer>(); //ITelemetryBuffer is used the channel
            Container.Register<ITelemetryTransmitter, AITelemetryTransmitter>(); //ITelemetryTransmitter transmits a block of telemetry to Applicatoin Insights.

            //setup the objects needed to serialize telemetry as part of transmission.
            Container.Register<IContextTagKeys, AIContextTagKeys>(); //Defines context tags expected by Application Insights.
            Container.Register<ITelemetrySerializer, AITelemetrySerializer>(); //Serialize telemetry items into a compressed Gzip data.
            Container.Register<IJsonWriterFactory, JsonWriterFactory>(); //Factory to create JSON converters as needed.
        }

        /// <summary>
        /// Flag to capture execution execution performance metrics using request telemetry.
        /// </summary>
        public bool TrackExecutionPerformance { get; set; }

        /// <summary>
        /// Flag to force flushing the telementry sink buffer after handler execution completes.
        /// </summary>
        public bool FlushTelemetryAfterExecution { get; set; }


        public virtual bool ConfigureTelemetrySink(ILocalWorkflowActivityContext localContext)
        {
            if (localContext != null)
            {
                var key = localContext.ExtensionSettings.Get<string>("Telemetry.InstrumentationKey");
               
                if (!string.IsNullOrEmpty(key))
                {
                    localContext.TracingService.Trace("Retrieved Telemetry Instrumentation Key.");
                    TelemetrySink.ProcessChain.TelemetryProcessors.Add(new SequencePropertyProcessor());
                    TelemetrySink.ProcessChain.TelemetryProcessors.Add(new InstrumentationKeyPropertyProcessor(key));
                    
                    return true; //telemetry sink is configured.
                }
            }

            return false; //telmetry sink is not configured.
        }

        protected override void Execute(CodeActivityContext codeActivityContext)
        {
            if (codeActivityContext == null) { throw new ArgumentNullException("codeActivityContext"); }

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var success = true;
            var responseCode = "200";

            var tracingService = codeActivityContext.GetExtension<ITracingService>();

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));

            var telemetryFactory = Container.Resolve<ITelemetryFactory>();
            var telemetryClientFactory = Container.Resolve<ITelemetryClientFactory>();

            var executionContext = codeActivityContext.GetExtension<IWorkflowContext>();

            //Create a dictionary of custom properties based on values in the execution context.
            var propertyManager = Container.Resolve<IXrmTelemetryPropertyManager>();
            var properties = propertyManager.CreateXrmPropertiesDictionary(this.GetType().ToString(), executionContext);

            //generate a telemetry client capturing all execution context information. Values
            //that do not map cleanly to telmentry context are kept in the custom properties
            //list.
            using (var telemetryClient = telemetryClientFactory.BuildClient(
                 this.GetType().ToString(),
                 this.TelemetrySink,
                 properties))
            {

                #region Setup Telementry Context

                telemetryClient.Context.Operation.Name = executionContext.MessageName;
                telemetryClient.Context.Operation.CorrelationVector = executionContext.CorrelationId.ToString();
                telemetryClient.Context.Operation.Id = executionContext.OperationId.ToString();
                telemetryClient.Context.Session.Id = executionContext.CorrelationId.ToString();
                         
                //use data context if supported.
                var asDataContext = telemetryClient.Context as ISupportDataKeyContext;
                if (asDataContext != null)
                {
                    asDataContext.Data.RecordSource = executionContext.OrganizationName;
                    asDataContext.Data.RecordType = executionContext.PrimaryEntityName;
                    asDataContext.Data.RecordId = executionContext.PrimaryEntityId.ToString();
                }
                
                #endregion

                try
                {
                    var localContextFactory = Container.Resolve<ILocalWorkflowActivityContextFactory>();

                    using (var localContext = localContextFactory.BuildLocalWorkflowActivityContext(executionContext, Container, codeActivityContext, telemetryClient))
                    {
                        if (!TelemetrySink.IsConfigured)
                        {
                            TelemetrySink.OnConfigure = () => { return this.ConfigureTelemetrySink(localContext); };
                        }

                        try
                        {
                            ExecuteInternal(localContext);
                        }
                        catch (InvalidWorkflowException ex)
                        {
                            success = false;
                            responseCode = "400"; //business rule exception
                            if (telemetryClient != null && telemetryFactory != null)
                            {
                                telemetryClient.Track(telemetryFactory.BuildMessageTelemetry(ex.Message, eSeverityLevel.Error));
                            }
                            throw;
                        }
                        catch (InvalidPluginExecutionException ex)
                        {
                            success = false;
                            responseCode = "400"; //business rule exception
                            if (telemetryClient != null && telemetryFactory != null)
                            {
                                telemetryClient.Track(telemetryFactory.BuildMessageTelemetry(ex.Message, eSeverityLevel.Error));
                            }
                            throw;
                        }
                        catch (Exception ex)
                        {
                            success = false;
                            responseCode = "500"; //Unexpected exception.
                            if (telemetryClient != null && telemetryFactory != null)
                            {
                                telemetryClient.Track(telemetryFactory.BuildExceptionTelemetry(ex));
                            }
                            throw;
                        }
                        finally
                        {
                            if (this.TrackExecutionPerformance && telemetryFactory != null && telemetryClient != null)
                            {
                                var r = telemetryFactory.BuildRequestTelemetry("WorkflowExecution", null, new Dictionary<string, string> { { "handlerName", "ExecuteInternal" } });
                                r.Duration = sw.Elapsed;
                                r.ResponseCode = responseCode;
                                r.Success = success;

                                telemetryClient.Track(r);
                            }

                            if (this.FlushTelemetryAfterExecution && telemetryClient != null)
                            {
                                telemetryClient.Flush();
                            }

                            sw.Stop();
                        }

                    } //using localContext
                }
                catch (InvalidPluginExecutionException ex)
                {
                    if (tracingService != null)
                    {
                        tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                    }
                    throw;
                }
                catch (InvalidWorkflowException ex)
                {   if (tracingService != null)
                    {
                        tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                    }
                    throw new InvalidPluginExecutionException(ex.Message, ex); //wrap the invalid workflow exception in an invalid plugin execution exception.
                }
                catch (Exception ex)
                {                    
                    if (tracingService != null)
                    {
                        tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                    }
                    throw new InvalidPluginExecutionException(string.Format("Unhandled Workflow Exception {0}", ex.Message), ex);
                }                

            } //using telemetryClient

            sw.Stop();
            sw = null;
        }
    }
}
