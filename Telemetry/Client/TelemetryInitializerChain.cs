using System.Collections.Generic;

namespace CCLLC.Telemetry.Client
{
    public class TelemetryInitializerChain : ITelemetryInitializerChain
    {
        public ICollection<ITelemetryInitializer> TelemetryInitializers { get; private set; }

        public TelemetryInitializerChain()
        {
            this.TelemetryInitializers = new List<ITelemetryInitializer>();
        }

        public void Initialize(ITelemetry telemetryItem)
        {
            foreach(var initializer in this.TelemetryInitializers)
            {
                initializer.Initialize(telemetryItem);
            }
        }
    }
}
