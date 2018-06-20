using System;
using System.Activities;
using System.Runtime.CompilerServices;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Diagnostics
{
    using Telemetry;

    public interface IDiagnosticService : ITelemetryContext, IDisposable 
    {
        ITelemetryProvider TelemetryProvider { get; }
    }
}
