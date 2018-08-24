using CCLLC.Core;

namespace CCLLC.Xrm.Sdk.Workflow
{
    
    public interface IWorkflowActivity
    {
        IIocContainer Container { get; }              
        void RegisterContainerServices();       
        void ExecuteInternal(ILocalWorkflowActivityContext context);
       
    }
}
