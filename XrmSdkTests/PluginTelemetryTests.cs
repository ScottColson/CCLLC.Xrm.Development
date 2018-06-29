using System;
using System.Collections.Generic;
using CCLLC.Telemetry;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace XrmSdkTests
{
    [TestClass]
    public class PluginTelemetryTests
    {
        
        public void Telemetry_Has_Expected_Properties()
        {
            var xrmFake = new XrmFakedContext();
            
            var plugin = new InstrumentedPlugin();                       
            plugin.TestingDelegate = (localContext) => 
            {
                localContext.Trace("This is a message");
            };

            xrmFake.ExecutePluginWith<InstrumentedPlugin>(getPluginExecutionContext(xrmFake), plugin);
            
            var items = new List<ITelemetry>(plugin.TelemetrySink.Channel.Buffer.Dequeue());
            Assert.AreEqual(1, items.Count);

            var telemetry = items[0] as IMessageTelemetry;
            Assert.IsNotNull(telemetry);

            var context = telemetry.Context;
            var executionContext = xrmFake.GetDefaultPluginContext();

            Assert.AreEqual(executionContext.MessageName, context.Operation.Name);
            //Bug with the fake is causing the execution context correltion id to reset back to an empty guid.
            //Assert.AreEqual(executionContext.CorrelationId.ToString(), context.Operation.CorrelationVector);
            //Assert.AreEqual(executionContext.CorrelationId.ToString(), context.Session.Id);
            Assert.AreEqual(executionContext.OperationId.ToString(), context.Operation.Id);
            Assert.AreEqual("XrmSdkTests.InstrumentedPlugin", telemetry.Properties["crm-pluginclass"]);
            Assert.AreEqual("20", telemetry.Properties["crm-stage"]);
            Assert.AreEqual("1", telemetry.Properties["crm-depth"]);
            Assert.AreEqual(executionContext.InitiatingUserId.ToString(), telemetry.Properties["crm-initiatinguser"]);
            Assert.AreEqual(executionContext.IsInTransaction.ToString(), telemetry.Properties["crm-isintransaction"]);
            Assert.AreEqual(executionContext.IsolationMode.ToString(), telemetry.Properties["crm-isolationmode"]);
            Assert.AreEqual(executionContext.Mode.ToString(), telemetry.Properties["crm-mode"]);
            Assert.AreEqual(executionContext.OrganizationId.ToString(), telemetry.Properties["crm-organizationid"]);
            Assert.AreEqual(executionContext.RequestId.ToString(), telemetry.Properties["crm-requestid"]);
            Assert.AreEqual(executionContext.UserId.ToString(), telemetry.Properties["crm-userid"]);
            Assert.AreEqual(executionContext.OrganizationName, telemetry.Properties["crm-recordsource"]);
            Assert.AreEqual(executionContext.PrimaryEntityId.ToString(), telemetry.Properties["crm-primaryentityid"]);
            Assert.AreEqual(executionContext.PrimaryEntityName, telemetry.Properties["crm-primaryentityname"]);

        }

        private XrmFakedPluginExecutionContext getPluginExecutionContext(XrmFakedContext xrmFake)
        {            
            var pluginContext = xrmFake.GetDefaultPluginContext();
            pluginContext.CorrelationId = Guid.NewGuid();
            pluginContext.Depth = 2;
            pluginContext.InitiatingUserId = Guid.NewGuid();
            pluginContext.IsInTransaction = true;
            pluginContext.IsolationMode = 1;
            pluginContext.MessageName = "Create";
            pluginContext.OperationId = Guid.NewGuid();
            pluginContext.OrganizationId = Guid.NewGuid();
            pluginContext.OrganizationName = "MyOrgName";
            pluginContext.PrimaryEntityId = Guid.NewGuid();
            pluginContext.PrimaryEntityName = "contact";
            pluginContext.Stage = 20;
            pluginContext.RequestId = Guid.NewGuid();
            pluginContext.UserId = Guid.NewGuid();

            return pluginContext;
        }
    }
}
