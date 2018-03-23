using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions.Context
{

    using Telemetry;

    public interface ILocalPluginContext<E, T> : ILocalContext<E,T> where E : Entity where T : ITelemetryService
    {
    }
}
