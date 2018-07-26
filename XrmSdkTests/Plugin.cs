using Microsoft.Xrm.Sdk;
using CCLLC.Xrm.Sdk;

namespace XrmSdkTests
{
    public class Plugin : PluginBase, IPlugin
    {
        public Plugin(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
            base.RegisterEventHandler(null, null, ePluginStage.PostOperation, ExecuteHandler);
        }

        public void ExecuteHandler(ILocalContext localContext)
        {
            //test basic telemetry
            localContext.Trace("Simple trace message.");
            localContext.Trace(eMessageType.Warning, "Warning message.");
            localContext.TrackEvent("My Event Name");
            
        }
    }
}
