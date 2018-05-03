using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Client
{
    public class ApplicationTelemetryClientFactory : IApplicationTelemetryClientFactory
    {
        ITelemetryContext telemetryContext;

        ApplicationTelemetryClientFactory(ITelemetryContext context)
        {
            this.telemetryContext = context;
        }

        public IApplicationTelemetryClient BuildClient(string applicationName, ITelemetrySink telemetrySink, IDictionary<string, string> contextProperties = null)
        {
            return new ApplicationTelemetryClient(applicationName, telemetrySink, telemetryContext, contextProperties);
        }
    }
}
