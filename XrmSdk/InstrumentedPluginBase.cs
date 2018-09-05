using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CCLLC.Telemetry;
using CCLLC.Telemetry.EventLogger;
using CCLLC.Telemetry.Client;
using CCLLC.Telemetry.Context;
using CCLLC.Telemetry.Serializer;
using CCLLC.Telemetry.Sink;
using Microsoft.Xrm.Sdk;

namespace CCLLC.Xrm.Sdk
{
    /// <summary>
    /// Base plugin class for plugins using <see cref="IEnhancedPlugin"/> functionality with telemetry logging outside 
    /// of Dynamics 365. For external telemetry use <see cref="InstrumentedPluginBase"/>."/>
    /// </summary>
    public abstract class InstrumentedPluginBase : PluginBase, ISupportPluginInstrumentation
    {
               
        /// <summary>
        /// Provides a <see cref="ITelemetrySink"/> to recieve and process various 
        /// <see cref="ITelemetry"/> items generated during the execution of the 
        /// Plugin.
        /// </summary>
        public virtual ITelemetrySink TelemetrySink { get; private set; }
        
        /// <summary>
        /// Constructor for <see cref="InstrumentedPluginBase"/>.
        /// </summary>
        /// <param name="unsecureConfig"></param>
        /// <param name="secureConfig"></param>
        public InstrumentedPluginBase(string unsecureConfig, string secureConfig) 
            : base(unsecureConfig, secureConfig)
        {
            TelemetrySink = Container.Resolve<ITelemetrySink>();
        }

        /// <summary>
        /// Registration of services used by base plugin.
        /// </summary>
        public override void RegisterContainerServices()
        {
            base.RegisterContainerServices();

            //Telemetry issue event logger
            Container.Implement<IEventLogger>().Using<InertEventLogger>().AsSingleInstance();

            //setup the objects needed to create/capture telemetry items.
            Container.Implement<ITelemetryFactory>().Using<TelemetryFactory>().AsSingleInstance();  //ITelemetryFactory is used to create new telemetry items.
            Container.Implement<ITelemetryClientFactory>().Using<TelemetryClientFactory>().AsSingleInstance(); //ITelemetryClientFactory is used to create and configure a telemetry client.
            Container.Implement<IXrmTelemetryPropertyManager>().Using<Telemetry.ExecutionContextPropertyManager>().AsSingleInstance(); //Plugin property manager.
            Container.Implement<ITelemetryContext>().Using<TelemetryContext>(); //ITelemetryContext is a dependency for telemetry creation.
            Container.Implement<ITelemetryInitializerChain>().Using<TelemetryInitializerChain>(); //ITelemetryInitializerChain is a dependency for building a telemetry client.

            //setup the objects needed to buffer and send telemetry to Application Insights.
            Container.Implement<ITelemetrySink>().Using<TelemetrySink>(); //ITelemetrySink receives telemetry from one or more telemetry clients, processes it, buffers it, and transmits it.
            Container.Implement<ITelemetryProcessChain>().Using<TelemetryProcessChain>(); //ITelemetryProcessChain holds 0 or more processors that can modify the telemetry prior to transmission.
            Container.Implement<ITelemetryChannel>().Using<SyncMemoryChannel>(); //ITelemetryChannel provides the buffering and transmission. There is a sync and an asynch channel.
            Container.Implement<ITelemetryBuffer>().Using<TelemetryBuffer>(); //ITelemetryBuffer is used the channel
            Container.Implement<ITelemetryTransmitter>().Using<AITelemetryTransmitter>(); //ITelemetryTransmitter transmits a block of telemetry to Applicatoin Insights.

            //setup the objects needed to serialize telemetry as part of transmission.
            Container.Implement<IContextTagKeys>().Using<AIContextTagKeys>(); //Defines context tags expected by Application Insights.
            Container.Implement<ITelemetrySerializer>().Using<AITelemetrySerializer>(); //Serialize telemetry items into a compressed Gzip data.
            Container.Implement<IJsonWriterFactory>().Using<JsonWriterFactory>(); //Factory to create JSON converters as needed.
        }

        /// <summary>
        /// Flag to capture execution execution performance metrics using request telemetry.
        /// </summary>
        public bool TrackExecutionPerformance { get; set; }

        /// <summary>
        /// Flag to force flushing the telementry sink buffer after handler execution completes.
        /// </summary>
        public bool FlushTelemetryAfterExecution { get; set; }

        /// <summary>
        /// Telememetry Sink that gathers and transmits telemetry.
        /// </summary>
        /// <param name="localContext"></param>
        /// <returns></returns>
        public virtual bool ConfigureTelemetrySink(ILocalPluginContext localContext)
        {
            if (localContext != null)
            {
                var key = localContext.ExtensionSettings.Get<string>("Telemetry.InstrumentationKey");
               
                if (!string.IsNullOrEmpty(key))
                {
                    TelemetrySink.ProcessChain.TelemetryProcessors.Add(new SequencePropertyProcessor());
                    TelemetrySink.ProcessChain.TelemetryProcessors.Add(new InstrumentationKeyPropertyProcessor(key));
                    
                    return true; //telemetry sink is configured.
                }
            }

            return false; //telmetry sink is not configured.
        }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// Microsoft CRM plugins must be thread-safe and stateless. 
        /// </remarks>
        public override void Execute(IServiceProvider serviceProvider)
        {           
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var success = true;
            var responseCode = "200";

            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entering {0}.Execute()", this.GetType().ToString()));

            var executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            var telemetryFactory = Container.Resolve<ITelemetryFactory>();
            var telemetryClientFactory = Container.Resolve<ITelemetryClientFactory>();

            //Create a dictionary of custom properties based on values in the execution context.
            var propertyManager = Container.Resolve<IXrmTelemetryPropertyManager>();
            var properties = propertyManager.CreateXrmPropertiesDictionary(this.GetType().ToString(), executionContext);
            
            using (var telemetryClient = telemetryClientFactory.BuildClient(
                this.GetType().ToString(),
                this.TelemetrySink,
                properties))
            {

                #region Setup Telementry Context

                //capture execution context attributes that fit in telemetry context
                telemetryClient.Context.Operation.Name = executionContext.MessageName;
                telemetryClient.Context.Operation.CorrelationVector = executionContext.CorrelationId.ToString();
                telemetryClient.Context.Operation.Id = executionContext.OperationId.ToString();
                telemetryClient.Context.Session.Id = executionContext.CorrelationId.ToString();

                //Capture data contexty if the telemetry provider supports data key context.
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
                    var matchingHandlers = this.PluginEventHandlers
                        .Where(a => (int)a.Stage == executionContext.Stage
                            && (string.IsNullOrWhiteSpace(a.MessageName) || string.Compare(a.MessageName, executionContext.MessageName, StringComparison.InvariantCultureIgnoreCase) == 0)
                            && (string.IsNullOrWhiteSpace(a.EntityName) || string.Compare(a.EntityName, executionContext.PrimaryEntityName, StringComparison.InvariantCultureIgnoreCase) == 0));

                    if (matchingHandlers.Any())
                    {
                        var localContextFactory = Container.Resolve<ILocalPluginContextFactory>();

                        using (var localContext = localContextFactory.BuildLocalPluginContext(executionContext, serviceProvider, this.Container, telemetryClient))
                        {
                            if (!TelemetrySink.IsConfigured)
                            {
                                TelemetrySink.OnConfigure = () => { return this.ConfigureTelemetrySink(localContext); };
                            }

                            foreach (var handler in matchingHandlers)
                            {
                                try
                                {
                                    handler.PluginAction.Invoke(localContext);
                                }
                                catch (InvalidPluginExecutionException ex)
                                {
                                    success = false;
                                    responseCode = "400"; //indicates a business rule error
                                    if (telemetryClient != null && telemetryFactory != null)
                                    {
                                        telemetryClient.Track(telemetryFactory.BuildMessageTelemetry(ex.Message, eSeverityLevel.Error));
                                    }
                                    throw;
                                }
                                catch (Exception ex)
                                {
                                    success = false;
                                    responseCode = "500"; //indicates a server error
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
                                        var r = telemetryFactory.BuildRequestTelemetry("PluginExecution", null, new Dictionary<string, string> { { "handlerName", handler.Id } });
                                        r.Duration = sw.Elapsed;
                                        r.ResponseCode = responseCode;
                                        r.Success = success;

                                        telemetryClient.Track(r);
                                    }

                                    if (this.FlushTelemetryAfterExecution && telemetryClient != null)
                                    {
                                        telemetryClient.Flush();
                                    }

                                    sw.Restart();
                                    
                                }
                            }
                        } //using localContext
                    }
                }
                catch (InvalidPluginExecutionException ex)
                {                    
                    tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                    throw;
                }
                catch (Exception ex)
                {                    
                    tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                    throw new InvalidPluginExecutionException(string.Format("Unhandled Plugin Exception {0}", ex.Message), ex);
                }
                
                
            } //using telemetryClient.

            sw.Stop();
            sw = null;

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.GetType().ToString()));
        }
    }
}
