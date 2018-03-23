using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions.Diagnostics
{
    using Telemetry;

    public class DiagnosticServiceFactory<T> : IDiagnosticServiceFactory<T> where T : ITelemetryService
    {      

        public IDiagnosticService<T> CreateDiagnosticService(string pluginClassName, IExecutionContext executionContext, ITracingService tracingService, ITelemetryProvider<T> telemetryProvider) 
        {
            return new DiagnosticService<T>(pluginClassName, executionContext, tracingService, telemetryProvider);
        }
    }
}
