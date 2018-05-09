using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Workflow.Context
{
    public interface ILocalWorkflowActivityContextFactory     {
      
        ILocalWorkflowActivityContext<E> BuildLocalWorkflowActivityContext<E>(IWorkflowContext executionContext, IIocContainer container, CodeActivityContext codeActivityContext, IComponentTelemetryClient telemetrClient) where E : Entity;

    }
}
