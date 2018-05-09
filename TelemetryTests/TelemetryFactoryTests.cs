using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CCLCC.Telemetry;

namespace TelemetryTests
{
    [TestClass]
    public class TelemetryFactoryTests
    {
        [TestMethod]
        public void BuildMessageTelemetry()
        {
            var factory = new TelemetryFactory();

            var message = Guid.NewGuid().ToString();

            var telemetry = factory.BuildMessageTelemetry(message, SeverityLevel.Error);

            telemetry.Sanitize();

            Assert.IsInstanceOfType(telemetry, typeof(ITelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(ISupportProperties));
            Assert.IsInstanceOfType(telemetry, typeof(IMessageTelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(IDataModelTelemetry<IMessageDataModel>));

            Assert.AreEqual("Message", telemetry.TelemetryName);
            Assert.AreEqual(message, telemetry.Message);
            Assert.AreEqual(SeverityLevel.Error, telemetry.SeverityLevel);
            Assert.IsNotNull(telemetry.Properties);
            Assert.AreEqual(0, telemetry.Properties.Count);
        }

        [TestMethod]
        public void BuildMessageTelemetryWithProperties()
        {
            var factory = new TelemetryFactory();

            var message = Guid.NewGuid().ToString();
            var props = new Dictionary<string, string>();
            props.Add("key1", "value1");

            var telemetry = factory.BuildMessageTelemetry(message, SeverityLevel.Error, props);
            telemetry.Sanitize();

            Assert.IsInstanceOfType(telemetry, typeof(ITelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(ISupportProperties));
            Assert.IsInstanceOfType(telemetry, typeof(IMessageTelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(IDataModelTelemetry<IMessageDataModel>));

            Assert.AreEqual("Message", telemetry.TelemetryName);
            Assert.AreEqual(message, telemetry.Message);
            Assert.AreEqual(SeverityLevel.Error, telemetry.SeverityLevel);
            Assert.IsNotNull(telemetry.Properties);
            Assert.AreNotSame(props, telemetry.Properties);
            Assert.AreEqual(1, telemetry.Properties.Count);
            Assert.AreEqual("value1", telemetry.Properties["key1"]);
        }

        [TestMethod]
        public void BuildEventTelemetry()
        {
            var factory = new TelemetryFactory();

            var name = Guid.NewGuid().ToString();
            var props = new Dictionary<string, string>();
            props.Add("key1", "value1");

            var telemetry = factory.BuildEventTelemetry(name);
            telemetry.Sanitize();

            Assert.IsInstanceOfType(telemetry, typeof(ITelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(ISupportProperties));
            Assert.IsInstanceOfType(telemetry, typeof(ISupportMetrics));
            Assert.IsInstanceOfType(telemetry, typeof(IEventTelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(IDataModelTelemetry<IEventDataModel>));
            

            Assert.AreEqual("Event", telemetry.TelemetryName);
            Assert.AreEqual(name, telemetry.Name);
            Assert.IsNotNull(telemetry.Properties);
            Assert.AreEqual(0, telemetry.Properties.Count);
            Assert.IsNotNull(telemetry.Metrics);
            Assert.AreEqual(0, telemetry.Metrics.Count);
        }


        [TestMethod]
        public void BuildEventTelemetryWithProperties()
        {
            var factory = new TelemetryFactory();

            var name = Guid.NewGuid().ToString();
            var props = new Dictionary<string, string>();
            props.Add("key1", "value1");

            var telemetry = factory.BuildEventTelemetry(name, props);
            telemetry.Sanitize();

            Assert.IsInstanceOfType(telemetry, typeof(ITelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(ISupportProperties));
            Assert.IsInstanceOfType(telemetry, typeof(ISupportMetrics));
            Assert.IsInstanceOfType(telemetry, typeof(IEventTelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(IDataModelTelemetry<IEventDataModel>));

            Assert.AreEqual("Event", telemetry.TelemetryName);
            Assert.AreEqual(name, telemetry.Name);            
            Assert.IsNotNull(telemetry.Properties);
            Assert.AreNotSame(props, telemetry.Properties);
            Assert.AreEqual(1, telemetry.Properties.Count);
            Assert.AreEqual("value1", telemetry.Properties["key1"]);
            Assert.IsNotNull(telemetry.Metrics);
            Assert.AreEqual(0, telemetry.Metrics.Count);
        }

        [TestMethod]
        public void BuildEventTelemetryWithPropertiesAndMetrics()
        {
            var factory = new TelemetryFactory();

            var name = Guid.NewGuid().ToString();
            var props = new Dictionary<string, string>();
            props.Add("key1", "value1");
            var metrics = new Dictionary<string, double>();
            metrics.Add("key1", 2.456);


            var telemetry = factory.BuildEventTelemetry(name, props, metrics);
            telemetry.Sanitize();

            Assert.IsInstanceOfType(telemetry, typeof(ITelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(ISupportProperties));
            Assert.IsInstanceOfType(telemetry, typeof(ISupportMetrics));
            Assert.IsInstanceOfType(telemetry, typeof(IEventTelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(IDataModelTelemetry<IEventDataModel>));

            Assert.AreEqual("Event", telemetry.TelemetryName);
            Assert.AreEqual(name, telemetry.Name);
            Assert.IsNotNull(telemetry.Properties);
            Assert.AreEqual(1, telemetry.Properties.Count);
            Assert.AreEqual("value1", telemetry.Properties["key1"]);
            Assert.IsNotNull(telemetry.Metrics);
            Assert.AreEqual(1, telemetry.Metrics.Count);
            Assert.AreEqual(2.456, telemetry.Metrics["key1"]);
            Assert.AreNotSame(metrics, telemetry.Metrics);
        }

        [TestMethod]
        public void BuildExceptionTelemetryWithPropertiesAndMetrics()
        {
            var factory = new TelemetryFactory();

            var ex = new Exception("some error");

            var props = new Dictionary<string, string>();
            props.Add("key1", "value1");
            var metrics = new Dictionary<string, double>();
            metrics.Add("key1", 2.456);


            var telemetry = factory.BuildExceptionTelemetry(ex, props, metrics);
            telemetry.Sanitize();

            Assert.IsInstanceOfType(telemetry, typeof(ITelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(ISupportProperties));
            Assert.IsInstanceOfType(telemetry, typeof(ISupportMetrics));
            Assert.IsInstanceOfType(telemetry, typeof(IExceptionTelemetry));
            Assert.IsInstanceOfType(telemetry, typeof(IDataModelTelemetry<IExceptionDataModel>));

            Assert.AreEqual("Exception", telemetry.TelemetryName);
            Assert.AreEqual(ex.Message, telemetry.Exception.Message);
            Assert.IsNotNull(telemetry.Properties);
            Assert.AreEqual(1, telemetry.Properties.Count);
            Assert.AreEqual("value1", telemetry.Properties["key1"]);
            Assert.IsNotNull(telemetry.Metrics);
            Assert.AreEqual(1, telemetry.Metrics.Count);
            Assert.AreEqual(2.456, telemetry.Metrics["key1"]);
            Assert.AreNotSame(metrics, telemetry.Metrics);
        }
    }
}
