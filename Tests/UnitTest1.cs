using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using FakeXrmEasy;


namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var fakedContext = new XrmFakedContext();

            var target = new Entity("account") { Id = Guid.NewGuid() };

            var fakedPlugin = fakedContext.ExecutePluginWithTarget<TestHarness.BasePluginTestHarness>(target);



        }
    }
}
