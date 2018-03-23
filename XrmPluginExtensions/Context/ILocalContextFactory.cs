using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions.Context
{
    using Diagnostics;
    using Telemetry;


    public interface ILocalContextFactory<T> where T : ITelemetryService 
    {
        ILocalPluginContext<E,T> CreateLocalPluginContext<E>(IPluginExecutionContext pluginExecutionContext, IServiceProvider serviceProvider, IDiagnosticService<T> diagnosticService) where E : Entity;
    }
}
