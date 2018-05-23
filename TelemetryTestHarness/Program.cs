using System;
using System.Threading;
using CCLLC.Telemetry;
using CCLLC.Telemetry.Context;
using CCLLC.Telemetry.Client;
using CCLLC.Telemetry.Sink;
using CCLLC.Telemetry.Serializer;
using System.Runtime.Caching;

namespace TelemetryTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var container = new CCLLC.Core.IocContainer();
                container.Register<ITelemetryContext, TelemetryContext>();
                container.Register<ITelemetryClientFactory, TelemetryClientFactory>();
                container.Register<ITelemetryInitializerChain, TelemetryInitializerChain>();
                container.Register<ITelemetryChannel, SyncMemoryChannel>();
                container.Register<ITelemetryBuffer, TelemetryBuffer>();
                container.Register<ITelemetryTransmitter, TelemetryTransmitter>();                
                container.Register<ITelemetryProcessChain, TelemetryProcessChain>();
                container.Register<ITelemetrySink, TelemetrySink>();
                container.Register<IContextTagKeys, AIContextTagKeys>();
                container.Register<ITelemetrySerializer, AITelemetrySerializer>();
                container.Register<ITelemetryFactory, TelemetryFactory>();

               
                
                //setup the telemetry sink
                var sink = container.Resolve<ITelemetrySink>();
                sink.Channel.SendingInterval = new TimeSpan(0, 0, 5); //override sending interval to 5 seconds.


                //Delegate called the first time that telemetry is pushed to the sink. Configures the sink
                //with a destination address for the telemetry, and adds telementry processors to set the 
                //instrumentation key and manage the sequence number. 
                sink.OnConfigure = () =>
                {
                    sink.Channel.EndpointAddress = new Uri("https://dc.services.visualstudio.com/v2/track"); //Application Insights
                    sink.ProcessChain.TelemetryProcessors.Add(new SequencePropertyProcessor());
                    sink.ProcessChain.TelemetryProcessors.Add(new InstrumentationKeyPropertyProcessor("7a6ecb67-6c9c-4640-81d2-80ce76c3ca34"));
                                        
                    return true; //indicate that the delegate successfully configured the sink.
                };

                
               
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                //setup the client
                var clientFactory = container.Resolve<ITelemetryClientFactory>();
                var client = clientFactory.BuildClient("app1", sink);
                client.Context.Component.Version = "1.9.4565";
                client.Context.Cloud.RoleInstance = "role.instance";
                client.Context.Cloud.RoleName = "role.name";
                client.Context.Operation.Id = Guid.NewGuid().ToString();

               





                //Sample Exceptions with Call Stack
                var factory = container.Resolve<ITelemetryFactory>();
                for(int i=0; i < 1; i++)
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

                //Sample Dependency Telemetry
                var deptel = factory.BuildDependencyTelemetry("Web", "http://someplace", "somename", null);
                using (var op = client.StartOperation<IDependencyTelemetry>(deptel))
                {
                    op.Properties.Add("someopproperty", "abcde");
                }

                //Sample Request Telemetry
                var reqtel = factory.BuildRequestTelemetry("a source", new Uri("http://www.bing.com"));
                using (var op = client.StartOperation<IRequestTelemetry>(reqtel))
                {
                    op.Properties.Add("somereqprop", "qwert");                    
                }

                client.TelemetrySink.Channel.Flush();

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
