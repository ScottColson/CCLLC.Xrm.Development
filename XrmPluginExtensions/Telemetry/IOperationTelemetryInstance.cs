using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmBase.Telemetry
{
    public interface IOperationTelemetryInstance: IDisposable 
    {
        ITelemetryService TelemetryService { get; }
        string OperationName { get; set; }
        void AddProperty(string name, string value);
        void AddMetric(string name, double value);
    }
}
