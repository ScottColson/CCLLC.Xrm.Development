using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
{
    public interface ITelemetryClientFactory
    {
        IComponentTelemetryClient BuildClient(string applicationName, ITelemetrySink telemetrySink, IDictionary<string, string> contextProperties = null);
    }
}
