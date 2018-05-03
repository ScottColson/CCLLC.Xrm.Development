using System;
using System.Collections.Generic;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IApplicationTelemetryClient : ITelemetryClient, IDisposable
    {
        string ApplicationName { get; }
        string InstrumentationKey { get; }
        ITelemetrySink TelemetrySink { get; }   

    }
}
