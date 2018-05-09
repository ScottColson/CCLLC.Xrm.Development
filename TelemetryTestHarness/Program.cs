using System;
using CCLCC.Telemetry;
using CCLCC.Telemetry.Context;
using CCLCC.Telemetry.Client;
using CCLCC.Telemetry.Sink;
using CCLCC.Telemetry.Serializer;

namespace TelemetryTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var context = new TelemetryContext();
                context.InstrumentationKey = "7a6ecb67-6c9c-4640-81d2-80ce76c3ca34";
                context.Component.Name = "Test1";
                context.Cloud.RoleInstance = "role.instance";
                context.Cloud.RoleName = "role.name";

                var clientFactory = new ApplicationTelemetryClientFactory(context, new TelemetryInitializerChain());
                var client = clientFactory.BuildClient("app1", new TelemetrySink(new InMemoryChannel(new TelemetryBuffer(), new TelemetryTransmitter(new AITelemetrySerializer(new AIContextTagKeys()))), new TelemetryProcessChain()));
                client.Initializers.TelemetryInitializers.Add(new SequencePropertyInitializer());

                var factory = new TelemetryFactory();
                for(int i=1; i< 100; i++)
                {
                    var telemetry = factory.BuildMessageTelemetry(string.Format("Message {0}",i), SeverityLevel.Warning);
                    telemetry.Properties.Add("crm1", "testvalue");
                    telemetry.Properties.Add("increment", i.ToString());

                    client.Track(telemetry);
                }
                
                client.TelemetrySink.Channel.Flush();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                var x = Console.ReadKey();
            }

        }
    }
}
