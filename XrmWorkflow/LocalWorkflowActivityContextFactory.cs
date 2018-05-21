using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLLC.Core;
using CCLLC.Telemetry;
using CCLLC.Xrm.Sdk.Workflow.Context;

namespace CCLLC.Xrm.Sdk.Workflow
{  
    public class LocalWorkflowActivityContextFactory : ILocalWorkflowActivityContextFactory
    {             
        public ILocalWorkflowActivityContext<E> BuildLocalWorkflowActivityContext<E>(IWorkflowContext executionContext, IIocContainer container, CodeActivityContext codeActivityContext, IComponentTelemetryClient telemetryClient = null) where E : Entity
        {
            if (telemetryClient == null)
            {
                return new LocalWorkflowActivityContext<E>(codeActivityContext, container, executionContext);
            }

            return new InstrumentedWorkflowActivityContext<E>(codeActivityContext, container, executionContext, telemetryClient);
        }

    }
}
