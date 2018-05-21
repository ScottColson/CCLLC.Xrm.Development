using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    internal class AIConstants
    {
        internal const string TelemetryServiceEndpoint = "https://dc.services.visualstudio.com/v2/track";

        internal const string TelemetryNamePrefix = "Microsoft.ApplicationInsights.";

        internal const string DevModeTelemetryNamePrefix = "Microsoft.ApplicationInsights.Dev.";

        internal const int MaxExceptionCountToSave = 10;
    }
}
