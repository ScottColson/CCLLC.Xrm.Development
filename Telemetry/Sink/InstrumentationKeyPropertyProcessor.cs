using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Sink
{
    public class InstrumentationKeyPropertyProcessor : ITelemetryProcessor
    {
        private string key;

        public InstrumentationKeyPropertyProcessor(string instrumentationKey)
        {
            key = instrumentationKey;
        }

        public void Process(ITelemetry telemetryItem)
        {
            if (string.IsNullOrEmpty(telemetryItem.InstrumentationKey))
            {
                telemetryItem.InstrumentationKey = key;
            }
        }
    }
}
