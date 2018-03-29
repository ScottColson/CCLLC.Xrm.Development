using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace CCLCC.XrmBase.Context
{
    using Container;
    using Diagnostics;

    public class LocalWorkflowActivityContextFactory : ILocalWorkflowActivityContextFactory
    {      
        public ILocalPluginContext<E> CreateLocalPluginContext<E>(IPluginExecutionContext pluginExecutionContext, IContainer container, IServiceProvider serviceProvider, IDiagnosticService diagnosticService) where E : Entity 
        {
            return new LocalPluginContext<E>(container, serviceProvider, pluginExecutionContext, diagnosticService);
        }

        public ILocalWorkflowActivityContext<E> CreateLocalWorkflowActivityContext<E>(IWorkflowContext executionContext, IContainer container, CodeActivityContext codeActivityContext, IDiagnosticService diagnosticService) where E : Entity
        {
            return new LocalWorkflowActivityContext<E>(container, codeActivityContext, executionContext, diagnosticService);
        }

    }
}
