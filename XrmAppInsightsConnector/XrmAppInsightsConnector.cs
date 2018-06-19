using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLLC.Core;
using CCLLC.Telemetry;
using CCLLC.Telemetry.EventLogger;
using CCLLC.Telemetry.Context;
using CCLLC.Telemetry.Client;
using CCLLC.Telemetry.Sink;
using CCLLC.Telemetry.Serializer;


namespace CCLLC.Xrm.AppInsights
{
    public class XrmAppInsightsConnector : IXrmAppInsightsConnector
    {
        private IEventLogger logger;        
        protected ITelemetrySink Sink { get; private set; }

        protected IIocContainer Container { get; private set; }

        public XrmAppInsightsConnector()
        {
            // register all the individual telemetry componenets required in an IoC
            // container. 
            Container = new IocContainer();

            RegisterContainerServices();

            logger = Container.Resolve<IEventLogger>();

            // setup and configure the sink. 
            Sink = Container.Resolve<ITelemetrySink>();
            Sink.OnConfigure = () => { return true; }; //no special configuraiton required.
        }

        protected virtual void RegisterContainerServices() {

            Container.RegisterAsSingleInstance<IEventLogger, InertEventLogger>();

            Container.RegisterAsSingleInstance<ITelemetryFactory, TelemetryFactory>();
            Container.RegisterAsSingleInstance<ITelemetryClientFactory, TelemetryClientFactory>();
            Container.Register<ITelemetryInitializerChain, TelemetryInitializerChain>();
            Container.Register<ITelemetryContext, TelemetryContext>();

            Container.Register<ITelemetrySink, TelemetrySink>();
            Container.Register<ITelemetryProcessChain, TelemetryProcessChain>();
            Container.Register<ITelemetryChannel, SyncMemoryChannel>();
            Container.Register<ITelemetryBuffer, TelemetryBuffer>();
            Container.Register<ITelemetryTransmitter, AITelemetryTransmitter>();

            Container.Register<ITelemetrySerializer, AITelemetrySerializer>();
            Container.Register<IContextTagKeys, AIContextTagKeys>();  //Context tags known to Application Insights.
            Container.Register<IJsonWriterFactory, JsonWriterFactory>();
        }

        public IXrmAppInsightsClient BuildClient(string className, IExecutionContext executionContext, string instrumentationKey)
        {
            var clientFactory = Container.Resolve<ITelemetryClientFactory>();

            // create a new telemetry client and capture common xrm execution context properties that 
            // don't fit in telemetry context as custom telemetry context properties.
            var client = clientFactory.BuildClient(
                className,
                this.Sink,
                new Dictionary<string, string>{
                    { "crm-pluginclass", className },
                    { "crm-depth", executionContext.Depth.ToString() },
                    { "crm-initiatinguser", executionContext.InitiatingUserId.ToString() },
                    { "crm-isintransaction", executionContext.IsInTransaction.ToString() },
                    { "crm-isolationmode", executionContext.IsolationMode.ToString() },
                    { "crm-mode", executionContext.Mode.ToString() },
                    { "crm-organizationid", executionContext.OrganizationId.ToString() },
                    { "crm-requestid", executionContext.RequestId.ToString() },
                    { "crm-userid", executionContext.UserId.ToString() },
                    { "crm-recordsource", executionContext.OrganizationName },
                    { "crm-primaryentityid", executionContext.PrimaryEntityId.ToString() },
                    { "crm-primaryentityname", executionContext.PrimaryEntityName }});

            //capture execution context attributes that do fit in telemetry context
            client.Context.Operation.Name = executionContext.MessageName;
            client.Context.Operation.CorrelationVector = executionContext.CorrelationId.ToString();
            client.Context.Operation.Id = executionContext.OperationId.ToString();
            client.Context.Session.Id = executionContext.CorrelationId.ToString();

            //capture plugin execution context properties as telemetry ge as context properties. 
            var asPluginExecutionContext = executionContext as IPluginExecutionContext;
            if (asPluginExecutionContext != null)
            {
                client.Context.Properties.Add("crm-stage", asPluginExecutionContext.Stage.ToString());
            }

            //capture workflow context properties as telemetry context properties.
            var asWorkflowExecutionContext = executionContext as IWorkflowContext;
            if(asWorkflowExecutionContext != null)
            {
                client.Context.Properties.Add("crm-stagename", asWorkflowExecutionContext.StageName);
            }

            //set instrumentation key
            client.InstrumentationKey = instrumentationKey;

            client.Initializers.TelemetryInitializers.Add(new SequencePropertyInitializer());

            var telemetryFactory = Container.Resolve<ITelemetryFactory>();

            return new XrmAppInsightsClient(telemetryFactory, client, logger);
        }
    }
}
