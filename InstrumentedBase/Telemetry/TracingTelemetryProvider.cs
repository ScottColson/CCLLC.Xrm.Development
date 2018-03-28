
using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Telemetry
{
    internal class EventTelemetryProvider : ITelemetryProvider 
    {        
        public bool IsInitialized
        {
            get { return false; }
        }

        public Func<Dictionary<string, string>> ServiceProviderSettings { private get; set; }


        public ITelemetryService CreateTelemetryService(string pluginClassName, ITelemetryProvider TelemetryProvider, ITracingService tracingService, IExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }

        public void SetConfigurationCallback(ConfigureTelemtryProvider callback)
        {
            throw new NotImplementedException();
        }

        public void Track(ITelemetry telemetry)
        {
        }

       
    }
}
