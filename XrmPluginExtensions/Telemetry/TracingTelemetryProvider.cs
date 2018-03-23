
using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Telemetry
{
    internal class TracingTelemetryProvider : ITelemetryProvider<ITelemetryService> 
    {        
        public bool IsInitialized
        {
            get { return false; }
        }

        public Func<Dictionary<string, string>> ServiceProviderSettings { private get; set; }

        public ITelemetryService CreateTelemetryService(string pluginClassName, ITelemetryProvider<ITelemetryService> telemetryProvider, ITracingService tracingService, IExecutionContext executionContext)
        {
            return new TracingTelemetryService(pluginClassName, telemetryProvider, tracingService, executionContext);
        }

        public void Track(ITelemetry telemetry)
        {
            throw new NotImplementedException("Tracing Telemetry Provider Track function is not implemented.");
        }

     
    }
}
