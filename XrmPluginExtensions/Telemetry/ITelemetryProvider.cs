using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace CCLCC.XrmBase.Telemetry
{  
    public interface ITelemetryProvider
    {        
        void SetConfigurationCallback(ConfigureTelemtryProvider callback);

        void Configure(IDictionary<string, string> configurationData);

        bool IsInitialized { get; }

        ITelemetryService CreateTelemetryService(string pluginClassName, ITracingService tracingService, ITelemetryProvider TelemetryProvider, IExecutionContext executionContext);

        void OnServiceDispose();
        
        void Track(ITelemetry telemetry);

        void Flush();
    }
}
