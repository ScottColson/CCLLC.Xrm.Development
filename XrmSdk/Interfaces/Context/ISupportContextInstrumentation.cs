using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk
{
    public interface ISupportContextInstrumentation
    {
        IComponentTelemetryClient TelemetryClient { get; }
        ITelemetryFactory TelemetryFactory { get; }
        void SetAlternateDataKey(string name, string value);
        void TrackEvent(string name);
    }
}
