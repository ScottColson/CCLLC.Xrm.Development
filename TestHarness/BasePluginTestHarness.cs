using System;
using Microsoft.Xrm.Sdk;
using CCLCC.XrmPluginExtensions;
using CCLCC.XrmPluginExtensions.Telemetry;
using CCLCC.XrmPluginExtensions.Context;


namespace TestHarness
{
    [CrmPluginRegistration(MessageNameEnum.Create, "account", StageEnum.PostOperation, ExecutionModeEnum.Synchronous, null, "sample step",1, IsolationModeEnum.Sandbox)]
    public class BasePluginTestHarness : Plugin<Entity>, IPlugin
    {
        public BasePluginTestHarness() : base()
        {
            RegisterMessageHandler("account", MessageNames.Create, ePluginStage.PostOperation, ExecuteLocal);            
        }

        public void ExecuteLocal(ILocalContext<Entity> localContext) 
        {
            try
            {
                localContext.PluginCache.Add<string>("akey", "avalue", 15);
                localContext.DiagnosticService.Trace(localContext.PluginCache.Get<string>("akey"));
                localContext.DiagnosticService.Trace("{0} entered plugin", "some value");
                localContext.DiagnosticService.Telemetry.TrackTrace(eSeverityLevel.Information, "{0} telemetry", "somevalue");                
            }
            catch (Exception ex)
            {
                localContext.DiagnosticService.Telemetry.TrackException(ex);
            }
            
        }
    }
}
