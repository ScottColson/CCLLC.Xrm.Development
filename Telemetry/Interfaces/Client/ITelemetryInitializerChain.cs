using System.Collections.Generic;

namespace CCLCC.Telemetry.Interfaces
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
