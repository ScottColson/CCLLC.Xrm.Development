using System;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry.Interfaces;
using CCLCC.Xrm;
using CCLCC.Xrm.Context;

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

        public override void RegisterContainerServices(IIocContainer container)
        {
            base.RegisterContainerServices(container);
            //add additional services to container

        }

        public void ExecuteLocal(ILocalContext<Entity> localContext) 
        {
            var x = this.Container.Resolve<IBlockTelemetry>();

                localContext.TelemetryClient.Properties.Add("a new property", "a new value");
                localContext.PluginCache.Add<string>("akey", "avalue", 15);

               
            
            
        }
    }
}
