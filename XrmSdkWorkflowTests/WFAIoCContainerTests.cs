using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCLLC.Xrm.Sdk;
using CCLLC.Xrm.Sdk.Configuration;
using CCLLC.Xrm.Sdk.Encryption;
using CCLLC.Xrm.Sdk.Utilities;
using CCLLC.Xrm.Sdk.Workflow;
using CCLLC.Telemetry;
using CCLLC.Telemetry.EventLogger;
using CCLLC.Telemetry.Context;
using CCLLC.Telemetry.Client;
using CCLLC.Telemetry.Sink;
using CCLLC.Telemetry.Serializer;

using TestHelpers;

namespace XrmSdkWorkflowTests
{
    [TestClass]
    public class PluginIoCContainerTests
    {    
        [TestMethod]
        public void WFA_Container_IsSingleton()
        {
            var wfa1 = new WFA();
            var wfa2 = new WFA();
            
            Assert.IsNotNull(wfa1.Container);
            Assert.IsNotNull(wfa2.Container);

            Assert.AreSame(wfa1.Container, wfa2.Container);

        }

        [TestMethod]
        public void InstrumentedWFA_Container_IsSingleton()
        {
            var wfa1 = new InstrumentedWFA();
            var wfa2 = new InstrumentedWFA();
            
            Assert.IsNotNull(wfa1.Container);
            Assert.IsNotNull(wfa2.Container);

            Assert.AreSame(wfa1.Container, wfa2.Container);

        }

        [TestMethod]
        public void InstrumentedWFA_Container_IsNot_WFA_Container()
        {
            var wfa1 = new WFA();
            var wfa2 = new InstrumentedWFA();

            Assert.IsNotNull(wfa1.Container);
            Assert.IsNotNull(wfa2.Container);

            Assert.AreNotSame(wfa1.Container, wfa2.Container);

        }

        [TestMethod]
        public void WFA_Container_Initialization()
        {
            var plugin = new WFA();
            Assert.IsNotNull(plugin.Container);
            Assert.AreEqual(6,plugin.Container.Count);

            //verify expected concreate implementations for each registered interface.
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ICacheFactory, CacheFactory>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<IConfigurationFactory, ConfigurationFactory>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ILocalWorkflowActivityContextFactory, LocalWorkflowActivityContextFactory>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<IRijndaelEncryption, RijndaelEncryption>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<IExtensionSettingsConfig, DefaultExtensionSettingsConfig>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<IPluginWebRequestFactory, PluginHttpWebRequestFactory>());
        }

        [TestMethod]
        public void InstrumentedWFA_Container_Initialization()
        {
            var plugin = new InstrumentedWFA();
            Assert.IsNotNull(plugin.Container);
            Assert.AreEqual(19, plugin.Container.Count);

            //verify expected concreate implementations for each registered interface.
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ICacheFactory, CacheFactory>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<IConfigurationFactory, ConfigurationFactory>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ILocalWorkflowActivityContextFactory, LocalWorkflowActivityContextFactory>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<IRijndaelEncryption, RijndaelEncryption>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<IExtensionSettingsConfig, DefaultExtensionSettingsConfig>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<IPluginWebRequestFactory, PluginHttpWebRequestFactory>());

            //verify expected concrete implementation for telemetry support
            Assert.IsTrue(plugin.Container.IsRegisteredAs<IEventLogger,InertEventLogger>(true));
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ITelemetryFactory,TelemetryFactory>(true));            
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ITelemetryClientFactory,TelemetryClientFactory>(true));            
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ITelemetryContext, TelemetryContext>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ITelemetryInitializerChain, TelemetryInitializerChain>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ITelemetrySink,TelemetrySink>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ITelemetryProcessChain,TelemetryProcessChain>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ITelemetryChannel, SyncMemoryChannel>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ITelemetryBuffer, TelemetryBuffer>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ITelemetryTransmitter, AITelemetryTransmitter>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<IContextTagKeys,AIContextTagKeys>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<ITelemetrySerializer,AITelemetrySerializer>());
            Assert.IsTrue(plugin.Container.IsRegisteredAs<IJsonWriterFactory, JsonWriterFactory>());
        }
        
    }
}
