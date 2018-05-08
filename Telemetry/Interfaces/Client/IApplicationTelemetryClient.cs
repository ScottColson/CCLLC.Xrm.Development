using System;
using System.Collections.Generic;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IApplicationTelemetryClient : ITelemetryClient, IDisposable
    {
        string ApplicationName { get; set; }
        string InstrumentationKey { get; set; }
        ITelemetrySink TelemetrySink { get; }   
        ITelemetryContext Context { get; }
        ITelemetryInitializerChain Initializers { get; }
    }
}
