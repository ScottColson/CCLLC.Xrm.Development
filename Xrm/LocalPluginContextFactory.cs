using System;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;
using CCLCC.Xrm.Sdk.Context;

namespace CCLCC.Xrm.Sdk
{
    public class LocalPluginContextFactory : ILocalPluginContextFactory
    {
        public ILocalPluginContext<E> BuildLocalPluginContext<E>(IPluginExecutionContext pluginExecutionContext, IServiceProvider serviceProvider, IIocContainer container, IComponentTelemetryClient telemetryClient) where E : Entity 
        {
            return new LocalPluginContext<E>(serviceProvider, container, pluginExecutionContext, telemetryClient);
        }     

    }
}
