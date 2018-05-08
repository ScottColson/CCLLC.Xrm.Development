using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Client
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
