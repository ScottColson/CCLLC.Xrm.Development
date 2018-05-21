using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
