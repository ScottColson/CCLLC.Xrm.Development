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
        IDictionary<string, string> Properties { get; }
        IDictionary<string, double> Metrics { get; }
    }
}
