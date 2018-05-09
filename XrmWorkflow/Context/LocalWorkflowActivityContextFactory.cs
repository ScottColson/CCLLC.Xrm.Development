using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Workflow.Context
{  
    public class LocalWorkflowActivityContextFactory : ILocalWorkflowActivityContextFactory
    {             
        public ILocalWorkflowActivityContext<E> BuildLocalWorkflowActivityContext<E>(IWorkflowContext executionContext, IIocContainer container, CodeActivityContext codeActivityContext, IApplicationTelemetryClient telemetryClient) where E : Entity
        {
            return new LocalWorkflowActivityContext<E>(codeActivityContext, container, executionContext, telemetryClient);
        }

    }
}
