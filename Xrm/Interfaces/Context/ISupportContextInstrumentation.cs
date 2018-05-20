using CCLCC.Telemetry;

namespace CCLCC.Xrm.Sdk
{
    public interface ISupportContextInstrumentation
    {
        IComponentTelemetryClient TelemetryClient { get; }
        ITelemetryFactory TelemetryFactory { get; }
        void SetAlternateDataKey(string name, string value);

    }
}
