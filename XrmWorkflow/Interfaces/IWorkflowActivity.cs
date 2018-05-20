using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Sdk.Workflow
{
    
    public interface IWorkflowActivity<E> where E : Entity
    {
        IIocContainer Container { get; }              
        void RegisterContainerServices();       
        void ExecuteInternal(ILocalWorkflowActivityContext<E> context);
       
    }
}
