using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Diagnostics
{
    using Telemetry;

    public interface IDiagnosticServiceFactory
    {
        IDiagnosticService CreateDiagnosticService(string pluginClassName, IExecutionContext executionContext, ITracingService tracingService, ITelemetryProvider TelemetryProvider);
    }
}
