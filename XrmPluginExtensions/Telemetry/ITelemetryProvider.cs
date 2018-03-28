using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Telemetry
{  
    public interface ITelemetryProvider
    {
        void SetConfigurationCallback(ConfigureTelemtryProvider callback);

        bool IsInitialized { get; }

        ITelemetryService CreateTelemetryService(string pluginClassName, ITelemetryProvider TelemetryProvider, ITracingService tracingService, IExecutionContext executionContext);

        void Track(ITelemetry telemetry);
    }
}
