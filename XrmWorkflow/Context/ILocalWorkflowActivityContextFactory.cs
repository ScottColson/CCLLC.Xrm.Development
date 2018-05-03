using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLCC.Core;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Xrm.Workflow.Context
{
    using Telemetry;

    public interface ILocalWorkflowActivityContextFactory     {
      
        ILocalWorkflowActivityContext<E> BuildLocalWorkflowActivityContext<E>(IWorkflowContext executionContext, IIocContainer container, CodeActivityContext codeActivityContext, IApplicationTelemetryClient telemetrClient) where E : Entity;

    }
}
