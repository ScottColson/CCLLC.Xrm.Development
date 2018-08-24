using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;

namespace CCLLC.Xrm.Sdk
{    
    public interface ILocalWorkflowActivityContext : ILocalContext
    {
        CodeActivityContext CodeActivityContext { get; }
        IWorkflowContext WorkflowContext { get; }
    }
}
