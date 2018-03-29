using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace CCLCC.XrmBase.Context
{
    using Container;
    using Diagnostics;   

    public interface ILocalWorkflowActivityContextFactory     {
      
        ILocalWorkflowActivityContext<E> CreateLocalWorkflowActivityContext<E>(IWorkflowContext executionContext, IContainer container, CodeActivityContext codeActivityContext, IDiagnosticService diagnosticService) where E : Entity;

    }
}
