using Microsoft.Xrm.Sdk;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk.Workflow
{
    public interface ISupportWorkflowActivityInstrumentation
    {
        ITelemetrySink TelemetrySink { get; }
        bool ConfigureTelemetrySink(ILocalWorkflowActivityContext localContext);

        bool TrackExecutionPerformance { get; set; }

        bool FlushTelemetryAfterExecution { get; set; }
    }
}


