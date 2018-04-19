using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Context
{
    using Container;
    using Telemetry;

    public class LocalPluginContextFactory : ILocalPluginContextFactory
    {
        public ILocalPluginContext<E> CreateLocalPluginContext<E>(IPluginExecutionContext pluginExecutionContext, IContainer container, IServiceProvider serviceProvider, ITelemetryService telemetryService) where E : Entity 
        {
            return new LocalPluginContext<E>(serviceProvider, container, pluginExecutionContext, telemetryService);
        }     

    }
}
