using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Diagnostics
{
    using Telemetry;

    public class DiagnosticServiceFactory : IDiagnosticServiceFactory 
    {      

        public IDiagnosticService CreateDiagnosticService(string pluginClassName, IExecutionContext executionContext, ITracingService tracingService, ITelemetryProvider telemetryProvider) 
        {
            return new DiagnosticService(pluginClassName, executionContext, tracingService, telemetryProvider);
        }
    }
}
