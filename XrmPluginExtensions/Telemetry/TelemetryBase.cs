using System;
using System.Collections.Generic;

namespace CCLCC.XrmBase.Telemetry
{
    public abstract class TelemetryBase : ITelemetry
    {
        public string TelemetryType { get; private set; }

        public string Message { get; protected set; }

        public IDictionary<string, string> Properties { get; private set; }

        public IDictionary<string, double> Metrics { get; private set; }

        public string Id { get; private set; }

        public DateTimeOffset Timestamp { get; set; }

        internal TelemetryBase(string telememtryType, IDictionary<string, string> properties, IDictionary<string, double> metrics)
        {            
            this.Id = Guid.NewGuid().ToString();
            this.Timestamp = new DateTimeOffset(DateTime.UtcNow);
            this.TelemetryType = telememtryType;
            this.Properties = properties != null ? new Dictionary<string, string>(properties) : new Dictionary<string, string>();
            this.Metrics = metrics != null ? new Dictionary<string, double>(metrics) : new Dictionary<string, double>();
        }

      


    }
}
