using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLCC.Core;
using CCLCC.Telemetry;
using CCLCC.Xrm.Context;

namespace CCLCC.Xrm.Workflow.Context
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

        public LocalWorkflowActivityContext(CodeActivityContext codeActivityContext, IIocContainer container, IWorkflowContext executionContext, IApplicationTelemetryClient telemetryClient)
          : base(executionContext, container, telemetryClient)
        {
            this.CodeActivityContext = codeActivityContext;
        }

        protected override IOrganizationServiceFactory CreateOrganizationServiceFactory()
        {
            return  this.CodeActivityContext.GetExtension<IOrganizationServiceFactory>();
        }
    }
}
