using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface ISupportMetrics
    {
        IDictionary<string, double> Metrics { get; }
    }
}
