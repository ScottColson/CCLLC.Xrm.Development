using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmBase.Telemetry
{
    public class OperationTelemetry : TelemetryBase, IOperationTelemetry
    {     
        public bool? Success { get; private set; }   
        public TimeSpan Duration { get; private set; }

        internal OperationTelemetry(string operationName, TimeSpan duration, bool? success, IDictionary<string, string> properties, IDictionary<string, double> metrics) 
            : base("Operation", properties, metrics)
        {
            this.Message = operationName;
            this.Success = success;
            this.Duration = duration;
        }
    }
}
