using System;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk
{
    public interface IPluginWebRequestFactory
    {
        IPluginWebRequest BuildPluginWebRequest(Uri address, ITelemetryFactory telemetryFactory = null, ITelemetryClient telemetryClient = null);
    }
}
