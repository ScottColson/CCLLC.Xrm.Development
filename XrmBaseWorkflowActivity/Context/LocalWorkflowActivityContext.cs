using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace CCLCC.XrmBase.Context
{
    using Container;
    using Diagnostics;

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

        public LocalWorkflowActivityContext(IContainer container, CodeActivityContext codeActivityContext, IWorkflowContext executionContext, IDiagnosticService diagnosticService)
          : base(container,  executionContext, diagnosticService)
        {
            this.CodeActivityContext = codeActivityContext;
        }

        protected override IOrganizationServiceFactory CreateOrganizationServiceFactory()
        {
            return  this.CodeActivityContext.GetExtension<IOrganizationServiceFactory>();
        }
    }
}
