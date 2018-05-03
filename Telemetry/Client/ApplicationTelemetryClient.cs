using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Client
{
    public class ApplicationTelemetryClient : ApplicationTelemetryClientBase
    {
        internal ApplicationTelemetryClient(string applicationName, ITelemetrySink telemetrySink, IDictionary<string, string> contextProperties) : base(applicationName, telemetrySink, contextProperties)
        {
        }

        public override void Initialize(ITelemetry telemetry)
        {
            throw new NotImplementedException();
        }
    }
}
