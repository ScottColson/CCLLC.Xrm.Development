using System;
using Microsoft.Xrm.Sdk;

namespace D365.XrmPluginExtensions.Diagnostics
{
    public class DiagnosticServiceFactory : IDiagnosticServiceFactory
    {
       

        public IDiagnosticService CreateDiagnosticService(Type plugin, ITracingService tracingService, IExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }
    }
}
