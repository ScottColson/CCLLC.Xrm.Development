using System;
using Microsoft.Xrm.Sdk;
using CCLLC.Core;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk
{  

    public interface ILocalPluginContextFactory 
    {
        ILocalPluginContext<E> BuildLocalPluginContext<E>(IPluginExecutionContext executionContext, IServiceProvider serviceProvider, IIocContainer container, IComponentTelemetryClient telemetryClient) where E : Entity;

    }
}
