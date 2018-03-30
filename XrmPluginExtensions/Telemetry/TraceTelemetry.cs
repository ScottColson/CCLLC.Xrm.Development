using System.Collections.Generic;

namespace CCLCC.XrmBase.Telemetry
{
    public class TraceTelemetry : TelemetryBase, ITraceTelemetry
    {
        public eSeverityLevel SeverityLevel { get; private set; }

        internal TraceTelemetry(string message, eSeverityLevel severityLevel, Dictionary<string,string> properties) 
            : base("Trace", properties, null)
        {
            this.Message = message;
            this.SeverityLevel = severityLevel;
        }
    }
}
