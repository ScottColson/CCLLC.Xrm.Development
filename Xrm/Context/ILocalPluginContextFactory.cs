using System;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Context
{  

    public interface ILocalPluginContextFactory 
    {
        ILocalPluginContext<E> CreateLocalPluginContext<E>(IPluginExecutionContext executionContext, IServiceProvider serviceProvider, IIocContainer container, IApplicationTelemetryClient telemetryClient) where E : Entity;

    }
}
