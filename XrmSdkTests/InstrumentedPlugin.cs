using Microsoft.Xrm.Sdk;
using CCLLC.Xrm.Sdk;

namespace XrmSdkTests
{
    public class InstrumentedPlugin : InstrumentedPluginBase, IPlugin
    {
        public InstrumentedPlugin(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
            base.RegisterEventHandler(null, null, ePluginStage.PostOperation, ExecuteHandler);
        }

        public void ExecuteHandler(ILocalContext localContext)
        { 
            //test basic telemetry
            localContext.Trace("Simple trace message.");
            localContext.Trace(eMessageType.Warning, "Warning message.");

            //test enhanced telemetry
            var asInstrumentedContext = localContext as ISupportContextInstrumentation;
            if (asInstrumentedContext != null)
            {
                asInstrumentedContext.TrackEvent("My Event Name");
            }
            

            
        }
    }
}
