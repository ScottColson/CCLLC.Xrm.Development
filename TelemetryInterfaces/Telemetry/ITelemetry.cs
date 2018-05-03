using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface ITelemetry
    {
        string InstrumentationKey { get; }
        string Sequence { get; }
        ITelemetryContext Context { get; }
        DateTimeOffset Timestamp { get; }
        string TelemetryName { get; }
        void Sanitize();
    }
}
