using System;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Context
{
    public class LocalPluginContextFactory : ILocalPluginContextFactory
    {
        public ILocalPluginContext<E> CreateLocalPluginContext<E>(IPluginExecutionContext pluginExecutionContext, IServiceProvider serviceProvider, IIocContainer container, IApplicationTelemetryClient telemetryClient) where E : Entity 
        {
            return new LocalPluginContext<E>(serviceProvider, container, pluginExecutionContext, telemetryClient);
        }     

    }
}
