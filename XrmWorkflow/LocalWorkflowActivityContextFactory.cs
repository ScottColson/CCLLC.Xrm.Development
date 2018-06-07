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
        public ILocalWorkflowActivityContext BuildLocalWorkflowActivityContext(IWorkflowContext executionContext, IIocContainer container, CodeActivityContext codeActivityContext, IComponentTelemetryClient telemetryClient = null)
        {
            if (telemetryClient == null)
            {
                return new LocalWorkflowActivityContext(codeActivityContext, container, executionContext);
            }

            return new InstrumentedWorkflowActivityContext(codeActivityContext, container, executionContext, telemetryClient);
        }

        
    }
}
