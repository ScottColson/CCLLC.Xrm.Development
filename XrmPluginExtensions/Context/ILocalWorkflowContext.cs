using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions.Context
{
    using Telemetry;

    interface ILocalWorkflowContext<E> : ILocalContext<E> where E : Entity
    {
    }
}
