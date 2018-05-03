using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IOperationalTelemetryClient<T>: ITelemetryClient, IDisposable where T : IOperationalTelemetry
    {       
        string OperationName { get; set; }
        IDictionary<string, string> Properties { get; }
    }
}
