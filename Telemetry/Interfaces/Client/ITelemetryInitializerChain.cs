using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface ITelemetryInitializerChain
    {
        ICollection<ITelemetryInitializer> TelemetryInitializers { get; }
       
        /// <summary>
        /// Process telemetry item through the chain of Telemetry Initializers.
        /// </summary>
        /// <param name="telemetryItem"></param>
        void Initialize(ITelemetry telemetryItem);
    }
}
