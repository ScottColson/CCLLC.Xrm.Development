using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Telemetry
{
    public class TracingTelemetryService : TelemetryServiceBase
    { 
        public override bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public override bool WritesToPluginTracLog
        {
            get
            {
                return true;
            }
        }



        public TracingTelemetryService(string pluginClassName, ITelemetryProvider telemetryProvider, ITracingService tracingService, IExecutionContext executionContext) 
            : base(pluginClassName,telemetryProvider,tracingService, executionContext)
        {           
        }

       
        public override void TrackTrace(eSeverityLevel severityLevel, string message, params object[] args)
        {
            TracingService.Trace("{0}: {1}", severityLevel, string.Format(message, args));
        } 

        public override void TrackException(Exception exception)
        {
            TrackTrace(eSeverityLevel.Exception, "Unhandled Exception: {0}", exception.Message);
        }
    }
}
