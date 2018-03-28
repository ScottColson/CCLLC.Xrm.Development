using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions.Context
{

    using Telemetry;

    public interface ILocalPluginContext<E> : ILocalContext<E> where E : Entity 
    {
    }
}
