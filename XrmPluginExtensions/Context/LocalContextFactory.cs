using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions.Context
{
    using Diagnostics;
    using Telemetry;

    public class LocalContextFactory<T> : ILocalContextFactory<T> where T : ITelemetryService
    {      
        public ILocalPluginContext<E,T> CreateLocalPluginContext<E>(IPluginExecutionContext pluginExecutionContext, IServiceProvider serviceProvider, IDiagnosticService<T> diagnosticService) where E : Entity 
        {
            return new LocalPluginContext<E, T>(serviceProvider, pluginExecutionContext, diagnosticService);
        }
    }
}
