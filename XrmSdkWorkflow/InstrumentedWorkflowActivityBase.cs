using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLLC.Core;
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
        private static IIocContainer _container;
        private static object _containerLock = new object();
        private static ITelemetrySink _telemetrySink;
        private static object _sinkLock = new object();

        /// <summary>
        /// Provides an <see cref="IIocContainer"/> instance to register all objects used by the
        /// base workflow activity. This container uses a static implementation therefore all 
        /// workflow activities that use this base share the same container and therefore
        /// use the same concreate implementations registered in the container.
        /// </summary>
        public override IIocContainer Container
        {
            get
            {
                if (_container == null)
                {
                    lock (_containerLock)
                    {
                        if (_container == null)
                        {
                            _container = new IocContainer();
                            RegisterContainerServices();
                        }
                    }
                }

                return _container;
            }
        }

        /// <summary>
        /// Provides a <see cref="ITelemetrySink"/> to recieve and process various 
        /// <see cref="ITelemetry"/> items generated during the execution of the 
        /// WorkflowActivity. This sink uses a static implementation therefore all 
        /// workflow activities that use this base share the same sink which is more
        /// efficient than operating multiple sinks.
        /// </summary>
        public virtual ITelemetrySink TelemetrySink
        {
            get
            {
                if (_telemetrySink == null)
                {
                    lock (_sinkLock)
                    {
                        if (_telemetrySink == null)
                        {
                            _telemetrySink = Container.Resolve<ITelemetrySink>();
                        }
                    }

                }

                return _telemetrySink;
            }
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

            var tracingService = codeActivityContext.GetExtension<ITracingService>();

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));

            var telemetryFactory = Container.Resolve<ITelemetryFactory>();
            var telemetryClientFactory = Container.Resolve<ITelemetryClientFactory>();

            var executionContext = codeActivityContext.GetExtension<IWorkflowContext>();

            //generate a telemetry client capturing all execution context information. Values
            //that do not map cleanly to telmentry context are kept in the custom properties
            //list.
            using (var telemetryClient = telemetryClientFactory.BuildClient(
                 this.GetType().ToString(),
                 this.TelemetrySink,
                 new Dictionary<string, string>{
                    { "crm-pluginclass", this.GetType().ToString() },
                    { "crm-correlationid", executionContext.CorrelationId.ToString() },
                    { "crm-depth", executionContext.Depth.ToString() },
                    { "crm-initiatinguser", executionContext.InitiatingUserId.ToString() },
                    { "crm-isintransaction", executionContext.IsInTransaction.ToString() },
                    { "crm-isolationmode", executionContext.IsolationMode.ToString() },
                    { "crm-messagename", executionContext.MessageName },
                    { "crm-mode", executionContext.Mode.ToString() },
                    { "crm-operationid", executionContext.OperationId.ToString()},
                    { "crm-organizationid", executionContext.OrganizationId.ToString() },
                    { "crm-orgname", executionContext.OrganizationName },
                    { "crm-requestid", executionContext.RequestId.ToString() },
                    { "crm-stagename", executionContext.StageName },
                    { "crm-userid", executionContext.UserId.ToString() }
                 }))
            {

                #region Setup Telementry Context

                telemetryClient.Context.Operation.Name = executionContext.MessageName;
                telemetryClient.Context.Operation.CorrelationVector = executionContext.CorrelationId.ToString();
                telemetryClient.Context.Operation.Id = executionContext.OperationId.ToString();

                telemetryClient.Context.Session.Id = executionContext.CorrelationId.ToString();

                //not all telemetry context providers support data context. In that case
                //use custom properties.
                var asDataContext = telemetryClient.Context as ISupportDataKeyContext;
                if (asDataContext != null)
                {
                    asDataContext.Data.RecordSource = executionContext.OrganizationName;
                    asDataContext.Data.RecordType = executionContext.PrimaryEntityName;
                    asDataContext.Data.RecordId = executionContext.PrimaryEntityId.ToString();
                }
                else
                {
                    telemetryClient.Context.Properties.Add("crm-recordsource", executionContext.OrganizationName);
                    telemetryClient.Context.Properties.Add("crm-primaryentityid", executionContext.PrimaryEntityId.ToString());
                    telemetryClient.Context.Properties.Add("crm-primaryentityname", executionContext.PrimaryEntityName);
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
                            if (telemetryClient != null && telemetryFactory != null)
                            {
                                telemetryClient.Track(telemetryFactory.BuildMessageTelemetry(ex.Message, eSeverityLevel.Error));
                            }
                            throw;
                        }
                        catch (Exception ex)
                        {
                            if (telemetryClient != null && telemetryFactory != null)
                            {
                                telemetryClient.Track(telemetryFactory.BuildExceptionTelemetry(ex));
                            }
                            throw;
                        }

                    } //using localContext
                }
                catch (Exception ex)
                {
                    if (tracingService != null)
                    {
                        tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                    }
                    throw;
                }

            } //using telemetryClient
        }
    }
}
