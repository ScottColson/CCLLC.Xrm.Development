using System;
using System.Threading;
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
                var container = new CCLCC.Core.IocContainer();
                container.Register<ITelemetryContext, TelemetryContext>();
                container.Register<ITelemetryClientFactory, TelemetryClientFactory>();
                container.Register<ITelemetryInitializerChain, TelemetryInitializerChain>();
                container.Register<ITelemetryChannel, InMemoryChannel>();
                container.Register<ITelemetryBuffer, TelemetryBuffer>();
                container.Register<ITelemetryTransmitter, TelemetryTransmitter>();                
                container.Register<ITelemetryProcessChain, TelemetryProcessChain>();
                container.Register<ITelemetrySink, TelemetrySink>();
                container.Register<IContextTagKeys, AIContextTagKeys>();
                container.Register<ITelemetrySerializer, AITelemetrySerializer>();






                var sink = container.Resolve<ITelemetrySink>();
                sink.OnConfigure = () => 
                {
                    return true;
                };
                sink.Channel.SendingInterval = new TimeSpan(0, 0, 5); //override sending interval to 5 seconds.

                var clientFactory = container.Resolve<ITelemetryClientFactory>();

                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var client = clientFactory.BuildClient("app1", sink);
                client.Context.InstrumentationKey = "7a6ecb67-6c9c-4640-81d2-80ce76c3ca34";
                client.Context.Cloud.RoleInstance = "role.instance";
                client.Context.Cloud.RoleName = "role.name";
                
                //client.Initializers.TelemetryInitializers.Add(new SequencePropertyInitializer());

                var factory = new TelemetryFactory();
                for(int i=0; i< 15; i++)
                {
                    try
                    {
                        try
                        {
                            throw new ArgumentOutOfRangeException("paramname", 45, "this is out of range");
                        }
                        catch(Exception i1)
                        {
                            throw new Exception(i1.Message, i1);
                        }
                    }
                    catch(Exception ex)
                    {
                        //var telemetry = factory.BuildEventTelemetry(string.Format("Event {0}",i));
                        var telemetry = factory.BuildExceptionTelemetry(ex);
                        telemetry.Properties.Add("crm1", "testvalue");
                        telemetry.Properties.Add("increment", i.ToString());

                        client.Track(telemetry);
                    }
                    
                }
                             
                sw.Stop();
                Console.WriteLine(string.Format("Elapsed: {0}", sw.ElapsedMilliseconds));
                Console.WriteLine(string.Format("Buffer Length: {0}", sink.Channel.Buffer.Length));

                //wait for channel to send on sending interval
                int k = 0;
                while (sink.Channel.Buffer.Length > 0)
                {
                    Thread.Sleep(1000);
                    k++;
                    Console.WriteLine(k.ToString());
                }

                Console.WriteLine(string.Format("Buffer Length: {0}", sink.Channel.Buffer.Length));

            }
            catch(Exception ex)
            {              
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Press key to continue.");
                var x = Console.ReadKey();
            }

        }
    }
}
