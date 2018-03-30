using System;
using System.Collections.Generic;

namespace CCLCC.XrmBase.Telemetry
{ 
    public interface ITelemetry
    {
        string Id { get; }
        DateTimeOffset Timestamp { get; set; }
        string TelemetryType { get; }
        string Message { get; }
        IReadOnlyDictionary<string, string> Properties { get; }
        IReadOnlyDictionary<string, double> Metrics { get; }

    }
}
