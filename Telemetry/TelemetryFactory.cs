using System;
using System.Collections.Generic;
using CCLCC.Telemetry.DataContract;


namespace CCLCC.Telemetry
{
    using Context;

    public class TelemetryFactory : ITelemetryFactory
    {
        public IEventTelemetry BuildEventTelemetry(string name, IDictionary<string, string> telemetryProperties = null, IDictionary<string, double> telemetryMetrics = null)
        {
            return new EventTelemetry(name, new TelemetryContext(), new EventDataModel(), telemetryProperties, telemetryMetrics);
        }

        public IExceptionTelemetry BuildExceptionTelemetry(Exception ex, IDictionary<string, string> telemetryProperties = null, IDictionary<string, double> telemetryMetrics = null)
        {
            return new ExceptionTelemetry(ex, new TelemetryContext(), new ExceptionDataModel(), telemetryProperties, telemetryMetrics);
        }

        public IMessageTelemetry BuildMessageTelemetry(string message, SeverityLevel severityLevel, IDictionary<string, string> telemetryProperties = null)
        {
            return new MessageTelemetry(message, severityLevel, new TelemetryContext(), new MessageDataModel(), telemetryProperties);
        }

        public IDependencyTelemetry BuildDependencyTelemetry(string dependencyTypeName, string target, string dependencyName, string dependencyData, IDictionary<string, string> telemetryProperties = null, IDictionary<string, double> telemetryMetrics = null)
        {
            return new DependencyTelemetry(dependencyTypeName, target, dependencyName, dependencyData, new TelemetryContext(), new DependencyDataModel(), telemetryProperties, telemetryMetrics);
        }
    }        
}
