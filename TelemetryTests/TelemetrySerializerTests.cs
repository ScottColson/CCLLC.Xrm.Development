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
        public void AISerializes_To_Expected_JSON()
        {

            string expectedJSON = "{\"name\":\"Microsoft.ApplicationInsights.instkey.Message\",\"time\":\"0001-01-01T00:00:00.0000000Z\",\"seq\":\"seq0\",\"iKey\":\"instkey\",\"tags\":{\"ai.cloud.roleInstance\":\"role.intance\",\"ai.application.ver\":\"compname.version\"},\"data\":{\"baseType\":\"MessageData\",\"baseData\":{\"ver\":2,\"message\":\"message text\",\"severityLevel\":\"Warning\",\"properties\":{\"key1\":\"value1\"}}}}" + 
                                  "\r\n{\"name\":\"Microsoft.ApplicationInsights.instkey.Message\",\"time\":\"0001-01-01T00:00:00.0000000Z\",\"seq\":\"seq1\",\"iKey\":\"instkey\",\"tags\":{\"ai.cloud.roleInstance\":\"role.intance\",\"ai.application.ver\":\"compname.version\"},\"data\":{\"baseType\":\"MessageData\",\"baseData\":{\"ver\":2,\"message\":\"message text\",\"severityLevel\":\"Warning\",\"properties\":{\"key1\":\"value1\"}}}}" + 
                                  "\r\n{\"name\":\"Microsoft.ApplicationInsights.instkey.Message\",\"time\":\"0001-01-01T00:00:00.0000000Z\",\"seq\":\"seq2\",\"iKey\":\"instkey\",\"tags\":{\"ai.cloud.roleInstance\":\"role.intance\",\"ai.application.ver\":\"compname.version\"},\"data\":{\"baseType\":\"MessageData\",\"baseData\":{\"ver\":2,\"message\":\"message text\",\"severityLevel\":\"Warning\",\"properties\":{\"key1\":\"value1\"}}}}" + 
                                  "\r\n{\"name\":\"Microsoft.ApplicationInsights.instkey.Message\",\"time\":\"0001-01-01T00:00:00.0000000Z\",\"seq\":\"seq3\",\"iKey\":\"instkey\",\"tags\":{\"ai.cloud.roleInstance\":\"role.intance\",\"ai.application.ver\":\"compname.version\"},\"data\":{\"baseType\":\"MessageData\",\"baseData\":{\"ver\":2,\"message\":\"message text\",\"severityLevel\":\"Warning\",\"properties\":{\"key1\":\"value1\"}}}}" + 
                                  "\r\n{\"name\":\"Microsoft.ApplicationInsights.instkey.Message\",\"time\":\"0001-01-01T00:00:00.0000000Z\",\"seq\":\"seq4\",\"iKey\":\"instkey\",\"tags\":{\"ai.cloud.roleInstance\":\"role.intance\",\"ai.application.ver\":\"compname.version\"},\"data\":{\"baseType\":\"MessageData\",\"baseData\":{\"ver\":2,\"message\":\"message text\",\"severityLevel\":\"Warning\",\"properties\":{\"key1\":\"value1\"}}}}";


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
                telemetry.Sequence = string.Format("seq{0}",i);
                items.Add(telemetry);
            }
            
            var serializer = new AITelemetrySerializer(new JsonWriterFactory(), new AIContextTagKeys());
            var json = Encoding.UTF8.GetString(serializer.Serialize(items,false)); //do not use compression.

            Assert.AreEqual(expectedJSON, json);
        }
    }
}
