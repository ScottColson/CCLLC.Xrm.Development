
using Microsoft.Xrm.Sdk;
using System;

namespace D365.XrmPluginExtensions.Diagnostics
{
    public interface IDiagnosticServiceFactory
    {
        IDiagnosticService CreateDiagnosticService(Type plugin, ITracingService tracingService, IExecutionContext executionContext);
    }
}
