using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    public interface ITelemetry
    {
        string InstrumentationKey { get; set; }
        string Sequence { get; set; }
        ITelemetryContext Context { get; }
        DateTimeOffset Timestamp { get; set; }
        string TelemetryName { get; }
        void Sanitize();
    }
}
