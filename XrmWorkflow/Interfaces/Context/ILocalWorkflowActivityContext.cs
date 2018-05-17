using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace CCLCC.Xrm.Sdk
{    
    public interface ILocalWorkflowActivityContext<E> : ILocalContext<E> where E : Entity
    {
        CodeActivityContext CodeActivityContext { get; }
        IWorkflowContext WorkflowContext { get; }
    }
}
