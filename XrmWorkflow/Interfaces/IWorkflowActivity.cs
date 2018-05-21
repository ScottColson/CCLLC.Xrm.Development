using Microsoft.Xrm.Sdk;
using CCLLC.Core;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk.Workflow
{
    
    public interface IWorkflowActivity<E> where E : Entity
    {
        IIocContainer Container { get; }              
        void RegisterContainerServices();       
        void ExecuteInternal(ILocalWorkflowActivityContext<E> context);
       
    }
}
