using System;
using Microsoft.Xrm.Sdk;
using CCLLC.Core;
using CCLLC.Telemetry;
using CCLLC.Xrm.Sdk.Context;

namespace CCLLC.Xrm.Sdk
{
    public class LocalPluginContextFactory : ILocalPluginContextFactory
    {
        public ILocalPluginContext BuildLocalPluginContext(IPluginExecutionContext pluginExecutionContext, IServiceProvider serviceProvider, IIocContainer container, IComponentTelemetryClient telemetryClient = null)
        {
            if(telemetryClient == null)
            {
                return new LocalPluginContext(serviceProvider, container, pluginExecutionContext);
            }

            return new InstrumentedPluginContext(serviceProvider, container, pluginExecutionContext, telemetryClient);
        }     

    }
}
