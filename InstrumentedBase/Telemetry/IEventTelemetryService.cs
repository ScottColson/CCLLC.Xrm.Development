using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Telemetry
{
    using Telemetry;

    public interface IEventTelemetryService : ITelemetryService
    {
        void TrackEvent(string EventName, IDictionary<string, double> metrics = null, IDictionary<string, string> additionalProperties = null);

    }
}
