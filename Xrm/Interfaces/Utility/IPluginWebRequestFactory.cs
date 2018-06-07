using System;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk
{
    public interface IPluginWebRequestFactory
    {
        IPluginWebRequest BuildPluginWebRequest(Uri address, string dependencyName=null, ITelemetryFactory telemetryFactory = null, ITelemetryClient telemetryClient = null);
    }
}
