using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Telemetry
{  
    public interface ITelemetryProvider
    {
        void SetConfigurationCallback(ConfigureTelemtryProvider callback);

        bool IsInitialized { get; }

        ITelemetryService CreateTelemetryService(string pluginClassName, ITelemetryProvider TelemetryProvider, ITracingService tracingService, IExecutionContext executionContext);

        void Track(ITelemetry telemetry);
    }
}
