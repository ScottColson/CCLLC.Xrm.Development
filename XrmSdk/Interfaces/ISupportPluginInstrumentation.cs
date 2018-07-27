using Microsoft.Xrm.Sdk;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk
{
    public interface ISupportPluginInstrumentation
    {
        ITelemetrySink TelemetrySink { get; }
        bool ConfigureTelemetrySink(ILocalPluginContext localContext);

        bool TrackExecutionPerformance { get; set; }

        bool FlushTelemetryAfterExecution { get; set; }
    }
}
