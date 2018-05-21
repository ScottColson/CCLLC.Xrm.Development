using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLLC.Core;
using CCLLC.Xrm.Sdk.Context;


namespace CCLLC.Xrm.Sdk.Workflow.Context
{  
    public class LocalWorkflowActivityContext<E> : LocalContext<E>, ILocalWorkflowActivityContext<E> where E : Entity
    {
        public CodeActivityContext CodeActivityContext { get; private set; }

        public IWorkflowContext WorkflowContext
        {
            get
            {
                return (IWorkflowContext)base.ExecutionContext;
            }
        }

        public LocalWorkflowActivityContext(CodeActivityContext codeActivityContext, IIocContainer container, IWorkflowContext executionContext)
          : base(executionContext, container)
        {
            this.CodeActivityContext = codeActivityContext;
        }

        protected override IOrganizationServiceFactory CreateOrganizationServiceFactory()
        {
            return  this.CodeActivityContext.GetExtension<IOrganizationServiceFactory>();
        }

        protected override ITracingService CreateTracingService()
        {
            return this.CodeActivityContext.GetExtension<ITracingService>();            
        }
    }
}
