using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCLLC.Telemetry;
using CCLLC.Telemetry.Client;
using CCLLC.Telemetry.Context;
using CCLLC.Telemetry.EventLogger;
using CCLLC.Telemetry.Serializer;
using CCLLC.Telemetry.Sink;

namespace XrmAppInsightsConnectorTests
{
    [TestClass]
    public class AppInsightsConnectorTests
    {
        [TestMethod]
        public void IoCContainer_Contains_Expected_Registrations()
        {
            var connector = new TestableConnector(false); //do not override container registrations.

            Assert.AreEqual(13, connector.TestableContainer.Count);
            //verify expected concrete implementatino for telemetry support
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<IEventLogger, InertEventLogger>(true));
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<ITelemetryFactory, TelemetryFactory>(true));
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<ITelemetryClientFactory, TelemetryClientFactory>(true));
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<ITelemetryContext, TelemetryContext>());
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<ITelemetryInitializerChain, TelemetryInitializerChain>());
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<ITelemetrySink, TelemetrySink>());
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<ITelemetryProcessChain, TelemetryProcessChain>());
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<ITelemetryChannel, SyncMemoryChannel>());
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<ITelemetryBuffer, TelemetryBuffer>());
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<ITelemetryTransmitter, AITelemetryTransmitter>());
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<IContextTagKeys, AIContextTagKeys>());
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<ITelemetrySerializer, AITelemetrySerializer>());
            Assert.IsTrue(connector.TestableContainer.IsRegisteredAs<IJsonWriterFactory, JsonWriterFactory>());
        }
    }
}
