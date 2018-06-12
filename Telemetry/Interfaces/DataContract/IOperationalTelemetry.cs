using System;

namespace CCLLC.Telemetry
{
    public interface IOperationalTelemetry : ISupportProperties, ISupportMetrics
    {
        string Id { get; set; }
        string Name { get; set; }
        bool? Success { get; set; }
        TimeSpan Duration { get; set; }
    }
}
