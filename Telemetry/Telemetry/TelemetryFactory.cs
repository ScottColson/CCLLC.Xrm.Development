using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLCC.Telemetry.Interfaces;


namespace CCLCC.Telemetry.Telemetry
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
    }        
}
