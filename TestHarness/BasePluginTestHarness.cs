using System;
using Microsoft.Xrm.Sdk;
using CCLCC.XrmBase;
using CCLCC.XrmBase.Telemetry;
using CCLCC.XrmBase.Context;
using System.Collections.Generic;

namespace TestHarness
{
    [CrmPluginRegistration(MessageNameEnum.Create, "account", StageEnum.PostOperation, ExecutionModeEnum.Synchronous, null, "sample step",1, IsolationModeEnum.Sandbox)]
    public class BasePluginTestHarness : PluginBase<Entity>, IPlugin
    {
        public BasePluginTestHarness() : base()
        {
            RegisterMessageHandler("account", MessageNames.Create, ePluginStage.PostOperation, ExecuteLocal);            
        }

        public void ExecuteLocal(ILocalContext<Entity> localContext) 
        {
            try
            {
                using (var op = localContext.DiagnosticService.Telemetry.StartOperation("an op name"))
                {
                    op.AddProperty("op-prop1", "op-value1");
                    op.AddMetric("op-metric1", 2000);

                    localContext.PluginCache.Add<string>("akey", "avalue", 15);
                    localContext.DiagnosticService.Trace(localContext.PluginCache.Get<string>("akey"));
                    localContext.DiagnosticService.Trace("{0} entered plugin", "some value");
                    localContext.DiagnosticService.Telemetry.TrackTrace(eSeverityLevel.Information, "{0} telemetry", "somevalue");
                    localContext.DiagnosticService.Telemetry.TrackEvent("some event name");
                    localContext.DiagnosticService.Telemetry.TrackEvent("some event with metrics and properites", new Dictionary<string, string>() { { "prop1", "value1" } }, new Dictionary<string, double>() { { "metric1", 1.3456 } });
                    throw new Exception("This is an exception");
                }
            }
            catch (Exception ex)
            {
                localContext.DiagnosticService.Telemetry.TrackException(ex);
            }
            
        }
    }
}
