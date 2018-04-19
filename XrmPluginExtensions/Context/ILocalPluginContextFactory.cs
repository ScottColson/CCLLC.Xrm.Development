using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Context
{
    using Container;
    using Telemetry; 

    public interface ILocalPluginContextFactory 
    {
        ILocalPluginContext<E> CreateLocalPluginContext<E>(IPluginExecutionContext executionContext, IContainer container, IServiceProvider serviceProvider, ITelemetryService telemetryService) where E : Entity;

    }
}
