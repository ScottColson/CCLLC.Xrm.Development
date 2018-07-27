using Microsoft.Xrm.Sdk;
using CCLLC.Xrm.Sdk;
using CCLLC.Telemetry;
using System;
using CCLLC.Telemetry.Sink;

namespace XrmSdkTests
{
    public class InstrumentedPlugin : InstrumentedPluginBase, IPlugin
    {
        private bool _overrideChannel;

        public InstrumentedPlugin(string unsecureConfig, string secureconfig) : this(unsecureConfig, secureconfig, false) { }
        public InstrumentedPlugin() : this(null, null, true) { }

        public InstrumentedPlugin(string unsecureConfig, string secureConfig, bool overrideChannel = true) : base(unsecureConfig, secureConfig)
        {
            _overrideChannel = overrideChannel;
            TrackExecutionPerformance = true;
            FlushTelemetryAfterExecution = true;
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
            var key = "7a6ecb67-6c9c-4640-81d2-80ce76c3ca34";

            if (!string.IsNullOrEmpty(key))
            {
                TelemetrySink.ProcessChain.TelemetryProcessors.Add(new SequencePropertyProcessor());
                TelemetrySink.ProcessChain.TelemetryProcessors.Add(new InstrumentationKeyPropertyProcessor(key));

                return true; //telemetry sink is configured.
            }

            return false;
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
