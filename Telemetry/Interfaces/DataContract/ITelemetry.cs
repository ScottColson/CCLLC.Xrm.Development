using System;
using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface ITelemetry
    {
        string InstrumentationKey { get; set; }
        string Sequence { get; set; }
        ITelemetryContext Context { get; }
        DateTimeOffset Timestamp { get; set; }
        string TelemetryName { get; }
        IDictionary<string, string> GetTaggedData();
        void Sanitize();
    }
}
