
using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Telemetry
{
    internal class TracingTelemetryProvider : ITelemetryProvider 
    {
        private ConfigureTelemtryProvider configurationCallback;      

        public bool IsInitialized { get; private set; }        

        public ITelemetryService CreateTelemetryService(string pluginClassName, ITelemetryProvider telemetryProvider, ITracingService tracingService, IExecutionContext executionContext)
        {
            return new TracingTelemetryService(pluginClassName, telemetryProvider, tracingService, executionContext);
        }

        public void SetConfigurationCallback(ConfigureTelemtryProvider callback)
        {
            configurationCallback = callback;
        }

        public void Track(ITelemetry telemetry)
        {
            if (IsInitialized == false && configurationCallback != null)
            {
                configurationCallback(this);
                IsInitialized = true;
            }
           
            throw new NotImplementedException("Tracing Telemetry Provider Track function is not implemented.");
        }       
    }
}
