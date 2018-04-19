using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace CCLCC.XrmBase.Context
{
    using Container;
    using Telemetry;

    public class LocalWorkflowActivityContextFactory : ILocalWorkflowActivityContextFactory
    {      
        public ILocalPluginContext<E> CreateLocalPluginContext<E>(IPluginExecutionContext pluginExecutionContext, IContainer container, IServiceProvider serviceProvider, ITelemetryService telemetryService) where E : Entity 
        {
            return new LocalPluginContext<E>(serviceProvider, container, pluginExecutionContext, telemetryService);
        }

        public ILocalWorkflowActivityContext<E> CreateLocalWorkflowActivityContext<E>(IWorkflowContext executionContext, IContainer container, CodeActivityContext codeActivityContext, ITelemetryService telemetryService) where E : Entity
        {
            return new LocalWorkflowActivityContext<E>(codeActivityContext, container, executionContext, telemetryService);
        }

    }
}
