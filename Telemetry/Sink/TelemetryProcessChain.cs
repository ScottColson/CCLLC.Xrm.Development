using System.Collections.Generic;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Sink
{
    public class TelemetryProcessChain : ITelemetryProcessChain
    {
        public ICollection<ITelemetryProcessor> TelemetryProcessors { get; private set; }

        public TelemetryProcessChain()
        {
            this.TelemetryProcessors = new List<ITelemetryProcessor>();
        }
        public void Process(ITelemetry telemetryItem)
        {
            foreach(var processor in this.TelemetryProcessors)
            {
                processor.Process(telemetryItem);
            }
        }
    }
}
