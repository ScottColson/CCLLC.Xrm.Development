
using Microsoft.Xrm.Sdk;
using System;

namespace CCLCC.XrmPluginExtensions.Diagnostics
{
    using Telemetry;

    public interface IDiagnosticServiceFactory<T> where T : ITelemetryService
    {
        IDiagnosticService<T> CreateDiagnosticService(string pluginClassName, IExecutionContext executionContext, ITracingService tracingService, ITelemetryProvider<T> TelemetryProvider);
    }
}
