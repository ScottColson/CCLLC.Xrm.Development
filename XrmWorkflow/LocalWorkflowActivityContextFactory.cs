using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLCC.Core;
using CCLCC.Telemetry;
using CCLCC.Xrm.Sdk.Workflow.Context;

namespace CCLCC.Xrm.Sdk.Workflow
{  
    public class LocalWorkflowActivityContextFactory : ILocalWorkflowActivityContextFactory
    {             
        public ILocalWorkflowActivityContext<E> BuildLocalWorkflowActivityContext<E>(IWorkflowContext executionContext, IIocContainer container, CodeActivityContext codeActivityContext, IComponentTelemetryClient telemetryClient) where E : Entity
        {
            return new LocalWorkflowActivityContext<E>(codeActivityContext, container, executionContext, telemetryClient);
        }

    }
}
