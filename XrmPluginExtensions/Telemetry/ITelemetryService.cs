using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Telemetry
{
    public interface ITelemetryService : IDisposable
    {
        IReadOnlyDictionary<string,string> Properties { get; }

        bool WritesToPluginTracLog { get; }

        bool IsInitialized { get; }

        void AddProperty(string name, string value);

        ITelemetryProvider<ITelemetryService> TelemetryProvider { get; }

        void TrackTrace(eSeverityLevel severityLevel, string message, params object[] args);

        void TrackException(Exception exception);

    }
}
