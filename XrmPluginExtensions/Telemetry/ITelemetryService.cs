using System;
using System.Collections.Generic;

namespace CCLCC.XrmBase.Telemetry
{
    public interface ITelemetryService : IDisposable
    {
        IReadOnlyDictionary<string, string> Properties { get; }

        bool WritesToPluginTracLog { get; }

        bool IsInitialized { get; }

        void AddProperty(string name, string value);

        ITelemetryProvider TelemetryProvider { get; }

        void TrackTrace(eSeverityLevel severityLevel, string message, params object[] args);

        void TrackException(Exception exception);

        void TrackEvent(string eventName, IDictionary<string, string> eventProperties = null, IDictionary<string, double> eventMetrics = null);

        void TrackOperation(string operationName, TimeSpan duration, bool? success, IDictionary<string, string> operationProperties, IDictionary<string, double> operationMetrics);
        IOperationTelemetryInstance StartOperation(string operationName);
    }
}
