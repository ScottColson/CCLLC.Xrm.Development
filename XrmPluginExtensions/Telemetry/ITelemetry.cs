using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Telemetry
{ 
    public interface ITelemetry
    {
        string TelemetryType { get; }
        string Message { get; }
        IReadOnlyDictionary<string, string> Properties { get; }
        IReadOnlyDictionary<string, double> Metrics { get; }

    }
}
