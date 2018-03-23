using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Telemetry
{
    public interface ITelemetryProvider<T> where T : ITelemetryService
    {
        Func<Dictionary<string, string>> ServiceProviderSettings { set; }

        bool IsInitialized { get; }

        T CreateTelemetryService(string pluginClassName, ITelemetryProvider<T> TelemetryProvider, ITracingService tracingService, IExecutionContext executionContext);

        void Track(ITelemetry telemetry);
    }
}
