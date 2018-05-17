using System;
using System.Collections.Generic;

namespace CCLCC.Telemetry
{
    public interface ITelemetryFactory
    {
        IDependencyTelemetry BuildDependencyTelemetry(string dependencyTypeName, string target, string dependencyName, string dependencyData, IDictionary<string, string> telemetryProperties = null, IDictionary<string, double> telemetryMetrics = null);
        IEventTelemetry BuildEventTelemetry(string name, IDictionary<string,string> telemetryProperties = null, IDictionary<string,double> telemetryMetrics = null);

        IExceptionTelemetry BuildExceptionTelemetry(Exception ex, IDictionary<string, string> telemetryProperties = null, IDictionary<string, double> telemetryMetrics = null);

        IMessageTelemetry BuildMessageTelemetry(string message, SeverityLevel severityLevel, IDictionary<string, string> telemetryProperties = null);
        IRequestTelemetry BuildRequestTelemetry(string source, Uri url, IDictionary<string, string> telemetryProperties = null, IDictionary<string, double> telemetryMetrics = null);
    }
}
