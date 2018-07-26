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

            Container.RegisterAsSingleInstance<IXrmTelemetryPropertyManager, DefaultPluginPropertyManager>();
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
            //Create a dictionary of custom properties based on values in the execution context.
            var propertyManager = Container.Resolve<IXrmTelemetryPropertyManager>();
            var properties = propertyManager.CreateXrmPropertiesDictionary(className, executionContext);

            // create a new telemetry client populated with the execution context properties.
            var clientFactory = Container.Resolve<ITelemetryClientFactory>();            
            var client = clientFactory.BuildClient(className, this.Sink, properties);

            //capture execution context attributes that fit in telemetry context
            client.Context.Operation.Name = executionContext.MessageName;
            client.Context.Operation.CorrelationVector = executionContext.CorrelationId.ToString();
            client.Context.Operation.Id = executionContext.OperationId.ToString();
            client.Context.Session.Id = executionContext.CorrelationId.ToString();            

            //set instrumentation key
            client.InstrumentationKey = instrumentationKey;

            client.Initializers.TelemetryInitializers.Add(new SequencePropertyInitializer());

            var telemetryFactory = Container.Resolve<ITelemetryFactory>();

            return new XrmAppInsightsClient(telemetryFactory, client, logger);
        }
    }
}
