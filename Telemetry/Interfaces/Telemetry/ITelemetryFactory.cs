using System;
using System.Collections.Generic;

namespace CCLCC.Telemetry.Interfaces
{
    public interface ITelemetryFactory
    {
        

        IEventTelemetry BuildEventTelemetry(string name, IDictionary<string,string> telemetryProperties = null, IDictionary<string,double> telemetryMetrics = null);

        IExceptionTelemetry BuildExceptionTelemetry(Exception ex, IDictionary<string, string> telemetryProperties = null, IDictionary<string, double> telemetryMetrics = null);

        IMessageTelemetry BuildMessageTelemetry(string message, SeverityLevel severityLevel, IDictionary<string, string> telemetryProperties = null);
    }
}
