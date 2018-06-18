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
        private IIocContainer container;
        private ITelemetrySink sink;

        public XrmAppInsightsConnector()
        {
            // register all the individual telemetry componenets required in an IoC
            // container. 
            container = new IocContainer();

            container.RegisterAsSingleInstance<IEventLogger, InertEventLogger>();
                        
            container.RegisterAsSingleInstance<ITelemetryFactory, TelemetryFactory>();

            container.Register<ITelemetryClientFactory, TelemetryClientFactory>();
            container.Register<ITelemetryInitializerChain, TelemetryInitializerChain>();
            container.Register<ITelemetryContext, TelemetryContext>();

            container.RegisterAsSingleInstance<ITelemetrySink, TelemetrySink>();
            container.Register<ITelemetryProcessChain, TelemetryProcessChain>();
            container.Register<ITelemetryChannel, SyncMemoryChannel>();
            container.Register<ITelemetryBuffer, TelemetryBuffer>();
            container.Register<ITelemetryTransmitter, AITelemetryTransmitter>();

            container.RegisterAsSingleInstance<ITelemetrySerializer, AITelemetrySerializer>();
            container.Register<IContextTagKeys, AIContextTagKeys>();  //Context tags known to Application Insights.
            container.Register<IJsonWriterFactory, JsonWriterFactory>();

            logger = container.Resolve<IEventLogger>();

            // setup and configure the sink. 
            sink = container.Resolve<ITelemetrySink>();
            sink.OnConfigure = () => { return true; }; //no special configuraiton required.
        }

        public IXrmAppInsightsClient BuildClient(string className, IExecutionContext executionContext, string instrumentationKey)
        {
            var clientFactory = container.Resolve<ITelemetryClientFactory>();

            // create a new telemetry client and capture common xrm execution context properties that 
            // don't fit in telemetry context as custom telemetry context properties.
            var client = clientFactory.BuildClient(
                className,
                this.sink,
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

            var telemetryFactory = container.Resolve<ITelemetryFactory>();

            return new XrmAppInsightsClient(telemetryFactory, client, logger);
        }
    }
}
