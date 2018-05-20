using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLCC.Core;
using CCLCC.Telemetry;
using CCLCC.Xrm.Sdk.Context;


namespace CCLCC.Xrm.Sdk.Workflow.Context
{
    public class InstrumentedWorkflowActivityContext<E> : InstrumentedContext<E>, ILocalWorkflowActivityContext<E> where E : Entity
    {
        public CodeActivityContext CodeActivityContext { get; private set; }

        public IWorkflowContext WorkflowContext
        {
            get
            {
                return (IWorkflowContext)base.ExecutionContext;
            }
        }

        public InstrumentedWorkflowActivityContext(CodeActivityContext codeActivityContext, IIocContainer container, IWorkflowContext executionContext, IComponentTelemetryClient telemetryClient)
          : base(executionContext, container, telemetryClient)
        {
            this.CodeActivityContext = codeActivityContext;
        }

        protected override IOrganizationServiceFactory CreateOrganizationServiceFactory()
        {
            return this.CodeActivityContext.GetExtension<IOrganizationServiceFactory>();
        }

        protected override ITracingService CreateTracingService()
        {
            return this.CodeActivityContext.GetExtension<ITracingService>();
        }
    }
}

