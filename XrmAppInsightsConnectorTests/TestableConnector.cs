using CCLLC.Core;
using CCLLC.Telemetry;

namespace XrmAppInsightsConnectorTests
{
    public class TestableConnector : CCLLC.Xrm.AppInsights.XrmAppInsightsConnector
    {
        private bool _overrideChannel;

        protected override void RegisterContainerServices()
        {
            base.RegisterContainerServices();

            if (_overrideChannel)
            {
                //replace standard channel with the mock
                base.Container.Register<ITelemetryChannel, MockChannel>();
            }
        }

        public TestableConnector(bool overrideChannel = true): base()
        {
            _overrideChannel = overrideChannel;
        }

        public ITelemetrySink TestableSink { get { return base.Sink; } }

        public IIocContainer TestableContainer { get { return base.Container; } }
    }
}
