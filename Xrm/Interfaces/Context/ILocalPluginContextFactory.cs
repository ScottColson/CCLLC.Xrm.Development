using System;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Sdk
{  

    public interface ILocalPluginContextFactory 
    {
        ILocalPluginContext<E> BuildLocalPluginContext<E>(IPluginExecutionContext executionContext, IServiceProvider serviceProvider, IIocContainer container, IComponentTelemetryClient telemetryClient) where E : Entity;

    }
}
