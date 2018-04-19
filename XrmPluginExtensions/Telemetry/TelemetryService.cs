using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Telemetry
{
    public class TelemetryService : TelemetryContextBase, ITelemetryService
    {
       private ITracingService tracingService;

        public ITelemetryProvider TelemetryProvider { get; private set; }

        internal TelemetryService(string pluginClassName, ITracingService tracingService, ITelemetryProvider telemetryProvider, IExecutionContext executionContext)
            : base(null, null)
        {
            this.tracingService = tracingService;
            this.TelemetryProvider = telemetryProvider;

            AddProperty("crm-class-name", pluginClassName);
            AddProperty("crm-correlation-id", executionContext.CorrelationId.ToString());
            AddProperty("crm-organization-name", executionContext.OrganizationName);
            AddProperty("crm-entity-type", executionContext.PrimaryEntityName);
            AddProperty("crm-message-name", executionContext.MessageName);
            AddProperty("crm-execution-depth", executionContext.Depth.ToString());
            AddProperty("crm-isolation-mode", executionContext.IsolationMode.ToString());
            AddProperty("crm-initiating-user-id", executionContext.InitiatingUserId.ToString());
            AddProperty("crm-user-id", executionContext.UserId.ToString());
            AddProperty("crm-in-transaction",executionContext.IsInTransaction ? "true" : "false");
        }
             

        public override void Dispose()
        {        
            if(TelemetryProvider != null)
            {
                TelemetryProvider.OnServiceDispose();
            }
            TelemetryProvider = null;
            tracingService = null;
            base.Dispose();
        }

       
     
       
        public override void Track(ITelemetry telemetry)
        {
            if(tracingService != null)
            {                
                switch(telemetry.TelemetryType)
                {
                    case "Trace":
                        var trace = (TraceTelemetry)telemetry;
                        tracingService.Trace("{0}-{1}, {2}:{3}", trace.Timestamp, trace.TelemetryType, trace.SeverityLevel, trace.Message);
                        break;
                    case "Event":
                        var ev = (EventTelemetry)telemetry;
                        tracingService.Trace("{0}-{1}, {2}", ev.Timestamp, ev.TelemetryType, ev.Message);
                        break;
                    case "Exception":
                        var ex = (ExceptionTelemetry)telemetry;
                        tracingService.Trace("{0}-{1}, {2}:{3}", ex.Timestamp, ex.TelemetryType, ex.ExceptionType, ex.Message);
                        break;
                    case "Operation":
                        var op = (OperationTelemetry)telemetry;
                        tracingService.Trace("{0}-{1}, {2}: Duration={3}, Success={4}", op.Timestamp, op.TelemetryType, op.Message, op.Duration, op.Success);
                        break;
                }

            }

            if(TelemetryProvider != null)
            {
                TelemetryProvider.Track(telemetry);
            }
        }

        
    }
}
