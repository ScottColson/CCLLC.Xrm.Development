using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCLLC.Xrm.AppInsights;
using CCLLC.Telemetry;
using System.Collections.Generic;

namespace XrmAppInsightsConnectorTests
{
    [TestClass]
    public class AppInsightsClientTests
    {
        private string testKey = "mytestkey";
        private TestableConnector connector;
        private FakeXrmEasy.XrmFakedPluginExecutionContext pluginContext;

        [TestMethod]
        public void Telemetry_Has_Expected_Properties()
        {
            var client = getClient();

            client.Trace("This is a message");

            var items = new List<ITelemetry>(connector.TestableSink.Channel.Buffer.Dequeue());
            Assert.AreEqual(1, items.Count);

            var telemetry = items[0] as IMessageTelemetry;
            Assert.IsNotNull(telemetry);

            var context = telemetry.Context;

            Assert.AreEqual(pluginContext.MessageName, context.Operation.Name);
            Assert.AreEqual(pluginContext.CorrelationId.ToString(), context.Operation.CorrelationVector);
            Assert.AreEqual(pluginContext.OperationId.ToString(), context.Operation.Id);
            Assert.AreEqual(pluginContext.CorrelationId.ToString(), context.Session.Id);

            Assert.AreEqual(this.GetType().ToString(), telemetry.Properties["source"]);
            Assert.AreEqual("Pre-operation", telemetry.Properties["stage"]);
            Assert.AreEqual(pluginContext.Depth.ToString(), telemetry.Properties["depth"]);
            Assert.AreEqual(pluginContext.InitiatingUserId.ToString(), telemetry.Properties["initiatingUserId"]);
            Assert.AreEqual(pluginContext.IsInTransaction.ToString(), telemetry.Properties["isInTransaction"]);
            Assert.AreEqual(pluginContext.IsolationMode.ToString(), telemetry.Properties["isolationMode"]);
            Assert.AreEqual("Synchronus", telemetry.Properties["mode"]);
            Assert.AreEqual(pluginContext.OrganizationId.ToString(), telemetry.Properties["orgId"]);
            Assert.AreEqual(pluginContext.RequestId.ToString(), telemetry.Properties["requestId"]);
            Assert.AreEqual(pluginContext.UserId.ToString(), telemetry.Properties["userId"]);
            Assert.AreEqual(pluginContext.OrganizationName, telemetry.Properties["orgName"]);
            Assert.AreEqual(pluginContext.PrimaryEntityId.ToString(), telemetry.Properties["entityId"]);
            Assert.AreEqual(pluginContext.PrimaryEntityName, telemetry.Properties["entityName"]);
            
        }


        [TestMethod]
        public void Trace_Generates_Expected_Telemetry()
        {
            var client = getClient();

            client.Trace("This is a message");

            var items = new List<ITelemetry>(connector.TestableSink.Channel.Buffer.Dequeue());
            Assert.AreEqual(1, items.Count);

            var telemetry = items[0] as IMessageTelemetry;
            Assert.IsNotNull(telemetry);
            Assert.AreEqual(eSeverityLevel.Information, telemetry.SeverityLevel);
            Assert.AreEqual("This is a message", telemetry.Message);
            Assert.AreEqual(testKey, telemetry.InstrumentationKey);
            
        }

        [TestMethod]
        public void TraceWithSeverity_Generates_Expected_Telemetry()
        {
            var client = getClient();

            client.Trace(eMessageType.Warning, "{0}", "This is a message");

            var items = new List<ITelemetry>(connector.TestableSink.Channel.Buffer.Dequeue());
            Assert.AreEqual(1, items.Count);

            var telemetry = items[0] as IMessageTelemetry;
            Assert.IsNotNull(telemetry);
            Assert.AreEqual(eSeverityLevel.Warning, telemetry.SeverityLevel);
            Assert.AreEqual("This is a message", telemetry.Message);
            Assert.AreEqual(testKey, telemetry.InstrumentationKey);
            Assert.IsNotNull(telemetry.Sequence);

        }
        
        [TestMethod]
        public void TrackEvent_Generates_Expected_Telemetry()
        {
            var client = getClient();

            client.TrackEvent("ThisEventName");

            var items = new List<ITelemetry>(connector.TestableSink.Channel.Buffer.Dequeue());
            Assert.AreEqual(1, items.Count);

            var telemetry = items[0] as IEventTelemetry;
            Assert.IsNotNull(telemetry);
            Assert.AreEqual("ThisEventName", telemetry.Name);
            Assert.AreEqual(testKey, telemetry.InstrumentationKey);

        }

        [TestMethod]
        public void TrackException_Generates_Expected_Telemetry()
        {
            var client = getClient();

            client.TrackException(new Exception("My exception message."));

            var items = new List<ITelemetry>(connector.TestableSink.Channel.Buffer.Dequeue());
            Assert.AreEqual(1, items.Count);

            var telemetry = items[0] as IExceptionTelemetry;
            Assert.IsNotNull(telemetry);
            Assert.AreEqual("My exception message.", telemetry.Exception.Message);
            Assert.AreEqual(testKey, telemetry.InstrumentationKey);

        }


        [TestMethod]
        public void StartDependencyOperation_Generates_Expected_Telemetry()
        {
            var client = getClient();

            using (var op = client.StartDependencyOperation("TestWeb", "TestTarget", "TestName"))
            {                
                op.CompleteOperation(true);
            }

            var items = new List<ITelemetry>(connector.TestableSink.Channel.Buffer.Dequeue());
            Assert.AreEqual(1, items.Count);

            var telemetry = items[0] as IDependencyTelemetry;
            Assert.IsNotNull(telemetry);
            Assert.AreEqual("TestWeb", telemetry.DependencyType);
            Assert.AreEqual("TestTarget", telemetry.Target);
            Assert.AreEqual("TestName", telemetry.Name);
            Assert.AreEqual(testKey, telemetry.InstrumentationKey);

        }

        private IXrmAppInsightsClient getClient()
        {
            connector = new TestableConnector();
            var xrmFake = new FakeXrmEasy.XrmFakedContext();
            pluginContext = xrmFake.GetDefaultPluginContext();
            pluginContext.CorrelationId = Guid.NewGuid();
            pluginContext.Depth = 2;
            pluginContext.InitiatingUserId = Guid.NewGuid();
            pluginContext.IsInTransaction = true;
            pluginContext.IsolationMode = 1;
            pluginContext.MessageName = "Update";
            pluginContext.OperationId = Guid.NewGuid();
            pluginContext.OrganizationId = Guid.NewGuid();
            pluginContext.OrganizationName = "MyOrgName";
            pluginContext.PrimaryEntityId = Guid.NewGuid();
            pluginContext.PrimaryEntityName = "contact";
            pluginContext.Stage = 20;
            pluginContext.RequestId = Guid.NewGuid();
            pluginContext.UserId = Guid.NewGuid();

            return connector.BuildClient(this.GetType().ToString(), pluginContext, testKey);

        }
    }
}
