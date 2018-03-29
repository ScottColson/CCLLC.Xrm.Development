using System;
using System.Collections.Generic;

namespace CCLCC.XrmBase.Telemetry
{
    public interface ITelemetryService : IDisposable
    {
        IReadOnlyDictionary<string,string> Properties { get; }

        bool WritesToPluginTracLog { get; }

        bool IsInitialized { get; }

        void AddProperty(string name, string value);

        ITelemetryProvider TelemetryProvider { get; }

        void TrackTrace(eSeverityLevel severityLevel, string message, params object[] args);

        void TrackException(Exception exception);

        void TrackEvent(string EventName, IDictionary<string, double> metrics = null, IDictionary<string, string> additionalProperties = null);

    }
}
