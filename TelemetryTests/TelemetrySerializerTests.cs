using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCLLC.Telemetry.Client;
using CCLLC.Telemetry.Context;
using CCLLC.Telemetry;
using CCLLC.Telemetry.Serializer;
using CCLLC.Telemetry.DataContract;
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
            var items = new List<ITelemetry>();
            
            var context = new TelemetryContext();
            context.InstrumentationKey = "instkey";
            context.Component.Version = "compname.version";
            context.Cloud.RoleInstance = "role.intance";
            var initializer = new SequencePropertyInitializer();
            for (int i = 0; i < 5; i++)
            {
                var data = new MessageDataModel();
                var telemetry = new MessageTelemetry("message text", eSeverityLevel.Warning, context, data, props);
                initializer.Initialize(telemetry);
                telemetry.Sanitize();
                items.Add(telemetry);
            }
            
            

            var serializer = new AITelemetrySerializer(new AIContextTagKeys());
            var json = Encoding.UTF8.GetString(serializer.Serialize(items,false));

    
        }
    }
}
