using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    public interface ITelemetrySink
    {
        Func<bool> OnConfigure { get; set; }

        bool IsConfigured { get; }

        ITelemetryChannel Channel { get; }

        ITelemetryProcessChain ProcessChain { get; }

        void Process(ITelemetry telemetryItem);
       
    }
}
