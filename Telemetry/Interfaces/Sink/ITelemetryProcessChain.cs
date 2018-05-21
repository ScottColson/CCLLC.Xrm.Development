using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    public interface ITelemetryProcessChain
    {
        ICollection<ITelemetryProcessor> TelemetryProcessors { get; }

        /// <summary>
        /// Process telemetry item through the chain of Telemetry Processors.
        /// </summary>
        /// <param name="telemetryItem"></param>
        void Process(ITelemetry telemetryItem);

    }
}
