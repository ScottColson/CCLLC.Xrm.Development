using System.Collections.Generic;

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
