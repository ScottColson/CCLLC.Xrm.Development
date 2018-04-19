using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Telemetry.Providers
{
    public class DefaultTelemetryProvider : ITelemetryProvider 
    {
        private ConfigureTelemtryProvider configurationCallback;
        
        public bool IsInitialized { get; private set; }

        public void Configure(IDictionary<string, string> configurationData)
        {            
        }

        public ITelemetryService CreateTelemetryService(string pluginClassName, ITracingService tracingService, ITelemetryProvider telemetryProvider, IExecutionContext executionContext)
        {
            return new TelemetryService(pluginClassName, tracingService, telemetryProvider, executionContext);
        }

        public void Flush()
        {
        }

        public void OnServiceDispose()
        {
        }

        public void SetConfigurationCallback(ConfigureTelemtryProvider callback)
        {
        }

        public void Track(ITelemetry telemetry)
        {
        }       
    }
}
