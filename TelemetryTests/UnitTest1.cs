using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCLCC.Telemetry.Client;
using CCLCC.Telemetry.Context;
using CCLCC.Telemetry.Interfaces;
using CCLCC.Telemetry.Serializer;
using CCLCC.Telemetry.Telemetry;
using System.Collections.Generic;
using CCLCC.Telemetry.Sink;
namespace TelemetryTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var context = new TelemetryContext();
            context.InstrumentationKey = "7a6ecb67-6c9c-4640-81d2-80ce76c3ca34";
            context.Component.Name = "Test1";

            var clientFactory = new ApplicationTelemetryClientFactory(context, new TelemetryInitializerChain());
            var client = clientFactory.BuildClient("app1", new TelemetrySink(new InMemoryChannel(new TelemetryBuffer(), new TelemetryTransmitter(new TelemetrySerializer(new AIContextTagKeys()))),new TelemetryProcessChain()));
            

            var factory = new TelemetryFactory();
            var telemetry = factory.BuildMessageTelemetry("message text", SeverityLevel.Warning);                        
            telemetry.Properties.Add("crm1", "testvalue");
            
            client.Track(telemetry);
            client.TelemetrySink.Channel.Flush();


        }
    }
}
