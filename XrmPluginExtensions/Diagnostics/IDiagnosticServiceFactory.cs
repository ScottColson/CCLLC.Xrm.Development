
using Microsoft.Xrm.Sdk;
using System;

namespace CCLCC.XrmPluginExtensions.Diagnostics
{
    using Telemetry;

    public interface IDiagnosticServiceFactory
    {
        IDiagnosticService CreateDiagnosticService(string pluginClassName, IExecutionContext executionContext, ITracingService tracingService, ITelemetryProvider TelemetryProvider);
    }
}
