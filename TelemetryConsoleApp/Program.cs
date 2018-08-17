using System;
using System.Threading;
using CCLLC.Core;
using CCLLC.Telemetry;
using CCLLC.Telemetry.Client;
using CCLLC.Telemetry.Context;
using CCLLC.Telemetry.EventLogger;
using CCLLC.Telemetry.Serializer;
using CCLLC.Telemetry.Sink;

namespace TelemetryConsoleApp
{
    /// <summary>
    /// Demonstrates how to use CCLLC.Telemetry system to send telemetry to Application Insights. To use this sample
    /// program you will need to setup an Azure Application Insights service and provide the instrumentation key for 
    /// that service when executing the program.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Using an IoC container is not required but it makes it much easier to
            // create the various objects required for telemetry to work.
            var container = new IocContainer();

            //setup the telemetry event logger to capture any issues that happen inside
            //the telemetry module. These issues are not captured in ApplicationInsights.
            container.Register<IEventLogger, InertEventLogger>();

            //setup the objects needed to create/capture telemetry items.
            container.RegisterAsSingleInstance<ITelemetryFactory, TelemetryFactory>();  //ITelemetryFactory is used to create new telemetry items.
            container.RegisterAsSingleInstance<ITelemetryClientFactory, TelemetryClientFactory>(); //ITelemetryClientFactory is used to create and configure a telemetry client.
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
            container.Register<ITelemetrySerializer, AITelemetrySerializer>(); //Serialize telemetry items into a compressed Gzip data.
            container.Register<IJsonWriterFactory, JsonWriterFactory>(); //Factory to create JSON converters as needed.
            
            //setup the telemetry sink which lives as long as the application is running.
            var sink = container.Resolve<ITelemetrySink>();
            
            Console.Write("Enter the Application Insights Instrumentation Key value: ");
            var instrumentationKey = Console.ReadLine();
            Console.WriteLine();

            //Delegate called the first time that telemetry is pushed to the sink. Adds 
            //telementry processors to set the instrumentation key and manage the 
            //sequence number. 
            sink.OnConfigure = () =>
            {
                Console.WriteLine("Configuring Sink.");
                sink.ProcessChain.TelemetryProcessors.Add(new SequencePropertyProcessor());
                sink.ProcessChain.TelemetryProcessors.Add(new InstrumentationKeyPropertyProcessor(instrumentationKey));
                sink.Channel.SendingInterval = new TimeSpan(0, 0, 3); //override sending interval to 3 seconds.
                return true; //indicate that the delegate successfully configured the sink.
            };

            ConsoleKeyInfo keystroke;

            do
            {
                try
                {            
                    //setup stopwatch to evaluate the cost of sending telemetry.
                    var sw = new System.Diagnostics.Stopwatch();
                    sw.Start();

                    //setup telemetry factory. This is registered in the container as
                    //a single instance so each loop execution actually gets back the 
                    //same telemetry factory.
                    var factory = container.Resolve<ITelemetryFactory>();

                    //setup a client facotry. This is also a single instance registration
                    //so each loop gets back the same client factory.
                    var clientFactory = container.Resolve<ITelemetryClientFactory>();                    
                    
                    //Build the client
                    var client = clientFactory.BuildClient("app1", sink);
                    client.Context.Component.Version = "1.9.4565";
                    client.Context.Cloud.RoleInstance = "role.instance";
                    client.Context.Cloud.RoleName = "role.name";
                    client.Context.Operation.Id = Guid.NewGuid().ToString();

                    var split = sw.Elapsed;
                    Console.WriteLine("Setup client in {0}", split);

                    //Sample Trace Message Telemetry
                    sw.Restart();
                    var messageTelemetry = factory.BuildMessageTelemetry("MyMessage", eSeverityLevel.Information);
                    client.Track(messageTelemetry);

                    split = sw.Elapsed;
                    Console.WriteLine("Captured trace message in {0}", split);

                    //Sample Event Telemetry
                    sw.Restart();
                    var eventTelemetry = factory.BuildEventTelemetry("MyEventName");
                    client.Track(eventTelemetry);

                    split = sw.Elapsed;
                    Console.WriteLine("Captured event in {0}", split);

                    //Sample Exception Telemetry with Call Stack
                    sw.Restart();
                    try
                    {
                        try
                        {
                            throw new ArgumentOutOfRangeException("paramname", 45, "this is out of range");
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message, e);
                        }
                    }
                    catch (Exception ex)
                    {
                        var exceptionTelemetry = factory.BuildExceptionTelemetry(ex);
                        exceptionTelemetry.Properties.Add("crm1", "testvalue"); //add additional property to this telemetry 
                        client.Track(exceptionTelemetry);
                    }
                    split = sw.Elapsed;
                    Console.WriteLine("Captured exception in {0}", split);


                    //Sample Dependency Telemetry
                    sw.Restart();
                    var deptel = factory.BuildDependencyTelemetry("Web", "http://someplace", "somename", null);
                    using (var op = client.StartOperation<IDependencyTelemetry>(deptel))
                    {
                        op.Properties.Add("someopproperty", "abcde");
                        op.CompleteOperation(true); //completed successfully.
                    }
                    split = sw.Elapsed;
                    Console.WriteLine("Captured dependency in {0}", split);

                    //Sample Request Telemetry
                    sw.Restart();
                    var reqtel = factory.BuildRequestTelemetry("a source", new Uri("http://mycallingservice"));
                    using (var op = client.StartOperation<IRequestTelemetry>(reqtel))
                    {
                        op.Properties.Add("somereqprop", "qwert");
                        op.Track(factory.BuildMessageTelemetry("A message in context of the operation.",eSeverityLevel.Information));
                        op.CompleteOperation(true); //completed successfully
                    }
                    split = sw.Elapsed;
                    Console.WriteLine("Captured request in {0}", split);

                    sw.Stop();

                    Console.WriteLine("Sink buffer contains {0} items.", sink.Channel.Buffer.Length);
                    Console.Write("Waiting for sink to send...");
                    //wait for channel to send on sending interval
                    int k = 0;
                    while (sink.Channel.Buffer.Length > 0)
                    {
                        Thread.Sleep(1000);
                        k++;
                        Console.Write(".");
                    }

                    Console.WriteLine();
                    Console.WriteLine(string.Format("Transmission completed. Sink buffer contains {0} items.", sink.Channel.Buffer.Length));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Console.WriteLine("Press Q to quit, any oter key to capture another set of telemetry.");
                    keystroke = Console.ReadKey();
                    Console.WriteLine();
                }
            } while (keystroke.Key != ConsoleKey.Q);
        }
    }
}

