using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCLCC.Telemetry.Client;
using CCLCC.Telemetry.Context;
using CCLCC.Telemetry.Interfaces;
using CCLCC.Telemetry.Serializer;
using CCLCC.Telemetry.Telemetry;
using System.Collections.Generic;
using System.Text;

namespace TelemetryTests
{
    [TestClass]
    public class TelemetrySerializerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var props = new Dictionary<string, string>();
            props.Add("key1", "value1");

            var context = new TelemetryContext();
            context.InstrumentationKey = "instkey";
            context.Component.Name = "compname";
            var data = new MessageDataModel();
            var telemetry = new MessageTelemetry("message text", SeverityLevel.Warning, context, data, props);
            telemetry.Sanitize();

            var initializer = new SequencePropertyInitializer();
            initializer.Initialize(telemetry);
            var items = new List<ITelemetry>();
            items.Add(telemetry);
            

            var serializer = new TelemetrySerializer(new AIContextTagKeys());
            var json = Encoding.UTF8.GetString(serializer.Serialize(items,false));

    
        }
    }
}
