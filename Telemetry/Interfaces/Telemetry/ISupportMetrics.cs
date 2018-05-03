using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface ISupportMetrics
    {
        IDictionary<string, double> Metrics { get; }
    }
}
