using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Sink
{
    public class TelemetrySink : ITelemetrySink
    {
        public Action OnConfigure { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsInitialized => throw new NotImplementedException();

        public ITelemetryChannel Channel => throw new NotImplementedException();

        public ITelemetryProcessChain ProcessChain => throw new NotImplementedException();

        public void Process(ITelemetry telemetryItem)
        {
            throw new NotImplementedException();
        }
    }
}
