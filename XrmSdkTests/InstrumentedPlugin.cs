using Microsoft.Xrm.Sdk;
using CCLLC.Xrm.Sdk;
using CCLLC.Telemetry;
using System;

namespace XrmSdkTests
{
    public class InstrumentedPlugin : InstrumentedPluginBase, IPlugin
    {
        private bool _overrideChannel;

        public InstrumentedPlugin() : this(null, null, true) { }

        public InstrumentedPlugin(string unsecureConfig, string secureConfig, bool overrideChannel = true) : base(unsecureConfig, secureConfig)
        {
            _overrideChannel = overrideChannel;
            base.RegisterEventHandler(null, null, ePluginStage.PreOperation, ExecuteHandler);
        }

        public override void RegisterContainerServices()
        {
            base.RegisterContainerServices();

            if (_overrideChannel)
            {
                //replace standard channel with the mock
                base.Container.Register<ITelemetryChannel, TestHelpers.MockChannel>();
            }
        }

        public override bool ConfigureTelemetrySink(ILocalPluginContext localContext)
        {
            return true;
        }

        public void ExecuteHandler(ILocalPluginContext localContext)
        { 
            if(TestingDelegate != null)
            {
                TestingDelegate(localContext);
            }
        }

        public Action<ILocalPluginContext> TestingDelegate;

    }
}
