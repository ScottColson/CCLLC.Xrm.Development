using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmBase.Telemetry
{
    public class EventTelemetry : TelemetryBase, IEventTelemetry
    {
        internal EventTelemetry(string eventName, IDictionary<string,string> properties, IDictionary<string,double> metrics)
            : base("Event", properties, metrics)
        {
            this.Message = eventName;
        }
    }
}
