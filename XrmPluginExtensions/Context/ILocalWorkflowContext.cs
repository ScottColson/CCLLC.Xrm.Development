using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions.Context
{
    using Telemetry;

    interface ILocalWorkflowContext<E,T> : ILocalContext<E,T> where E : Entity where T : ITelemetryService
    {
    }
}
