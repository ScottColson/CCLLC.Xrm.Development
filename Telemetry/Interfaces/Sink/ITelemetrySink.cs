using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
{
    public interface ITelemetrySink
    {
        Action OnConfigure { get; set; }

        bool IsInitialized { get; }

        ITelemetryChannel Channel { get; }

        ITelemetryProcessChain ProcessChain { get; }

        void Process(ITelemetry telemetryItem);
       
    }
}
