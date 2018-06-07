using Microsoft.Xrm.Sdk;
using CCLLC.Core;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk.Workflow
{
    
    public interface IWorkflowActivity
    {
        IIocContainer Container { get; }              
        void RegisterContainerServices();       
        void ExecuteInternal(ILocalWorkflowActivityContext context);
       
    }
}
