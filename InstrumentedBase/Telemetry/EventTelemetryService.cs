using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Telemetry
{
    public class EventTelemetryService : ITelemetryService
    {
        public bool WritesToPluginTracLog => throw new NotImplementedException();

        public bool IsInitialized => throw new NotImplementedException();

        public ITelemetryProvider TelemetryProvider => throw new NotImplementedException();

        public IReadOnlyDictionary<string, string> Properties => throw new NotImplementedException();

        public void AddProperty(string name, string value)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void TrackEvent(string EventName, IDictionary<string, double> metrics = null, IDictionary<string, string> additionalProperties = null)
        {
            throw new NotImplementedException();
        }

        public void TrackException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void TrackTrace(eSeverityLevel level, string message, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
