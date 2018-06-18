using Microsoft.Xrm.Sdk;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.AppInsights
{
    public interface IXrmAppInsightsConnector
    {
        IXrmAppInsightsClient BuildClient(string className, IExecutionContext context, string instrumentationKey = null);
    }
}
