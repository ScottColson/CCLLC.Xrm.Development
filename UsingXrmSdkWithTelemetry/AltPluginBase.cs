using System;
using CCLLC.Xrm.Sdk;
using CCLLC.Telemetry;


namespace UsingXrmSdkWithTelemetry
{       
    /// <summary>
    /// This is an example of an plugin base class that provides an alternate
    /// TelemetrySink configuration from that provided by the <see cref="InstrumentedPluginBase"/>.
    /// </summary>
    public abstract class AltPluginBase : InstrumentedPluginBase
    {
        // all plugins that inherit from InstrumentedPluginBase use a single telmetry
        // sink. Since this alternate base class uses an alternate configuration for
        // the telemetry sink it cannot share the sink provided by InstrumentedPluginBase
        // so it must override the TelementrySink implementation. The sink is
        // implemented as a singleton so it survives as long a the plugin survives.
        private static ITelemetrySink _telemetrySink;
        private static object _sinkLock = new object();        
        public override ITelemetrySink TelemetrySink
        {
            get
            {
                if (_telemetrySink == null)
                {
                    lock (_sinkLock)
                    {
                        if (_telemetrySink == null)
                        {
                            _telemetrySink = Container.Resolve<ITelemetrySink>(); 
                        }
                    }
                }

                return _telemetrySink;
            }
        }


        public AltPluginBase(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
        }

        // Override ConfigureTelemetrySink to include a new custom TelemetryProcessor 
        // in addition to the two processors that the InstrumentedPluginBase uses.
        // With the exception of adding the new CustomPluginPropertyProcessor, this
        // is the same code used in InstrumentedPluginBase.
        public override bool ConfigureTelemetrySink(ILocalPluginContext localContext)
        {
            if (localContext != null)
            {
                var key = localContext.ExtensionSettings.Get<string>("Telemetry.InstrumentationKey");
                if (!string.IsNullOrEmpty(key))
                {
                    TelemetrySink.ProcessChain.TelemetryProcessors.Add(new CCLLC.Telemetry.Sink.SequencePropertyProcessor());
                    TelemetrySink.ProcessChain.TelemetryProcessors.Add(new CCLLC.Telemetry.Sink.InstrumentationKeyPropertyProcessor(key));

                    //this telemetry processor will stamp each telemetry item with datetime of when the sink was first configured.
                    TelemetrySink.ProcessChain.TelemetryProcessors.Add(new Telemetry.CustomPluginPropertyProcessor("sink-config-time", DateTime.UtcNow.ToString()));

                    return true; //telemetry sink is configured.
                }
            }

            return false; //telmetry sink is not configured.
        }       
       
    }
}
