using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Xrm.Workflow
{
    using Context;

    public interface IWorkflowActivity<E> where E : Entity
    {
        IIocContainer Container { get; }
        ITelemetrySink TelemetrySink { get; }           
        void RegisterContainerServices(IIocContainer container);       
        void ExecuteInternal(ILocalWorkflowActivityContext<E> context);
    }
}
