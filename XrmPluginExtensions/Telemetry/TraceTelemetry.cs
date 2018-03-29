using System.Collections.Generic;

namespace CCLCC.XrmBase.Telemetry
{
    public class TraceTelemetry : ITraceTelemetry
    {
        public string TelemetryType { get; private set; }

        public string Message { get; private set; }

        public eSeverityLevel SeverityLevel { get; private set; }

        public IReadOnlyDictionary<string, string> Properties { get; private set; }

        public IReadOnlyDictionary<string, double> Metrics { get; private set; } 


        internal TraceTelemetry(string message, eSeverityLevel severityLevel, Dictionary<string,string> properties)
        {
            this.TelemetryType = "Trace";
            this.Message = message;
            this.SeverityLevel = severityLevel;
            this.Properties = properties ?? new Dictionary<string, string>();
            this.Metrics = null;
        }
    }
}
