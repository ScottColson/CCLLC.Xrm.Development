using System;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.AppInsights
{
    public interface IXrmAppInsightsClient
    {
        ITelemetryClient TelemetryClient { get; }

        ITelemetryFactory TelemetryFactory { get; }

        IOperationalTelemetryClient<IDependencyTelemetry> StartDependencyOperation(string dependencyType = "Web", string target = "", string dependencyName = "PluginWebRequest");

        void Trace(string message, params object[] args);

        void Trace(eMessageType type, string message, params object[] args);

        void TrackEvent(string name);

        void TrackException(Exception ex);
    }
}
