using CCLLC.Core;
using CCLLC.Telemetry;
using CCLLC.Telemetry.Context;
using CCLLC.Telemetry.Client;
using CCLLC.Telemetry.Sink;
using CCLLC.Telemetry.Serializer;


namespace TelemetryConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Using an IoC container is not required but it makes it much easier to
            // create the various objects required for telemetry to work.
            var container = new IocContainer();

            //setup the objects needed to create/capture telemetry items.
            container.Register<ITelemetryFactory, TelemetryFactory>();  //ITelemetryFactory is used to create new telemetry items.
            container.Register<ITelemetryClientFactory, TelemetryClientFactory>(); //ITelemetryClientFactory is used to create and configure a telemetry client.
            container.Register<ITelemetryContext, TelemetryContext>(); //ITelemetryContext is a dependency for telemetry creation.
            container.Register<ITelemetryInitializerChain, TelemetryInitializerChain>(); //ITelemetryInitializerChain is a dependency for building a telemetry client.

            //setup the objects needed to buffer and send telemetry to Application Insights.
            container.Register<ITelemetrySink, TelemetrySink>(); //ITelemetrySink receives telemetry from one or more telemetry clients, processes it, buffers it, and transmits it.
            container.Register<ITelemetryProcessChain, TelemetryProcessChain>(); //ITelemetryProcessChain holds 0 or more processors that can modify the telemetry prior to transmission.
            container.Register<ITelemetryChannel, AsyncMemoryChannel>(); //ITelemetryChannel provides the buffering and transmission. There is a sync and an asynch channel.
            container.Register<ITelemetryBuffer, TelemetryBuffer>(); //ITelemetryBuffer is used the channel
            container.Register<ITelemetryTransmitter, AITelemetryTransmitter>(); //ITelemetryTransmitter transmits a block of telemetry to Applicatoin Insights.
            
            //setup the objects needed to serialize telemetry as part of transmission.
            container.Register<IContextTagKeys, AIContextTagKeys>(); //Defines context tags expected by Application Insights.
            container.Register<ITelemetrySerializer, AITelemetrySerializer>(); //
            container.Register<IJsonWriter, JsonWriter>();
            

        }
    }
}
