using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace CCLCC.XrmBase.Context
{
    using Container;
    using Telemetry;

    public interface ILocalWorkflowActivityContextFactory     {
      
        ILocalWorkflowActivityContext<E> CreateLocalWorkflowActivityContext<E>(IWorkflowContext executionContext, IContainer container, CodeActivityContext codeActivityContext, ITelemetryService telemetryService) where E : Entity;

    }
}
