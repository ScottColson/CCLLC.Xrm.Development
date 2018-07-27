using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using FakeXrmEasy;

namespace XrmSdkTests
{
    [TestClass]
    public class PluginExtensionSettingsTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var contactId = Guid.NewGuid();
            var fakedContext = new XrmFakedContext();
            
            //setup ctx to look like an update of parentcustomerid on the contact
            var plugCtx = fakedContext.GetDefaultPluginContext();
            plugCtx.MessageName = "Create";
            plugCtx.PrimaryEntityName = "contact";
            plugCtx.Stage = 20;
            plugCtx.PrimaryEntityId = contactId;
            plugCtx.InputParameters["Target"] = new Entity { Id = contactId, LogicalName = "contact" };
            
            fakedContext.ExecutePluginWithConfigurations<InstrumentedPlugin>(plugCtx, null, null);

        }
    }
}
