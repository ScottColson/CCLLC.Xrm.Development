using System;
using System.Collections.Generic;
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

        public override void TrackEvent(string eventName, IDictionary<string, string> eventProperties = null, IDictionary<string, double> eventMetrics = null)
        {
            TrackTrace(eSeverityLevel.Information, "Event: {0}", eventName);
        }

        public override void TrackOperation(string operationName, TimeSpan duration, bool? success, IDictionary<string, string> operationProperties, IDictionary<string, double> operationMetrics)
        {
            TrackTrace(eSeverityLevel.Information, "Operation: {0} - duration={1} milliseconds, success={2}", operationName, duration.TotalMilliseconds, success);
        }

        public override IOperationTelemetryInstance StartOperation(string operationName)
        {
            return new OperationTelemetryInstance(this, operationName);
        }
    }
}
