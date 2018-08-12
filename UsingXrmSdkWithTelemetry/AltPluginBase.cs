using System;
using CCLLC.Xrm.Sdk;


namespace UsingXrmSdkWithTelemetry
{
    /// <summary>
    /// This is an example of an plugin base class derived from
    /// <see cref="InstrumentedPluginBase"/>. This new base class 
    /// implements an alternate TelemetrySink configuration that uses
    /// an Telemetry Processor defined in this code base.
    /// </summary>
    public abstract class AltPluginBase : InstrumentedPluginBase
    {
        public AltPluginBase(string unsecureConfig, string secureConfig) 
            : base(unsecureConfig, secureConfig)
        {
            //Turn on performance tracking for all plugins derived from
            //this base. This will write execution time metrics to AI using
            //Request Telemetry.
            base.TrackExecutionPerformance = true;
        }

        // Override ConfigureTelemetrySink to include a new custom TelemetryProcessor 
        public override bool ConfigureTelemetrySink(ILocalPluginContext localContext)
        {
            if (base.ConfigureTelemetrySink(localContext))
            {
                //Add our custom telemetry processor which will stamp each telemetry 
                //item with datetime of when the sink was first configured.
                TelemetrySink.ProcessChain.TelemetryProcessors.Add(new Telemetry.CustomPluginPropertyProcessor("sink-config-time", DateTime.UtcNow.ToString()));

                return true;
            }            

            return false; //telmetry sink is not configured.
        }       
       
    }
}
