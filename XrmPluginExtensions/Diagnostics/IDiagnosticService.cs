using System;
using System.Activities;
using System.Runtime.CompilerServices;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Diagnostics
{
    using Telemetry;

    public interface IDiagnosticService : IDisposable 
    { 
        void EnterMethod([CallerMemberName]string methodname = "");

        void ExitMethod(string message = null);
      
        void Trace(string message, params object[] args);

        void TracePluginException(InvalidPluginExecutionException ex);

        void TraceWorkflowException(InvalidWorkflowException ex);

        void TraceGeneralException(Exception ex);

        ITelemetryService Telemetry { get; }
    }
}
