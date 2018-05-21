using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCLLC.Xrm;

namespace XrmPluginTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var context = new FakeXrmEasy.XrmFakedContext();

            var pluginCtx = context.GetDefaultPluginContext();
            pluginCtx.MessageName = "Create";
            pluginCtx.Stage = 40;

            context.ExecutePluginWithConfigurations<XrmPluginTestHarness.SamplePlugin>(pluginCtx, null, null);

        }
    }
}
