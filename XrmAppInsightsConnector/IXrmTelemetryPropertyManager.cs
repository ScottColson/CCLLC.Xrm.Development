using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace CCLLC.Xrm.AppInsights
{
    public interface IXrmTelemetryPropertyManager
    {
        IDictionary<string, string> CreateXrmPropertiesDictionary(string className, IExecutionContext executionContext);

    }
}
