using System;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk.Utilities
{
    public class PluginHttpWebRequestFactory : IPluginWebRequestFactory
    {
        public IPluginWebRequest BuildPluginWebRequest(Uri address, string dependencyName = null, ITelemetryFactory telemetryFactory = null, ITelemetryClient telemetryClient = null)
        {
            return new PluginHttpWebRequest(address, dependencyName,  telemetryFactory, telemetryClient);
        }
    }
}
