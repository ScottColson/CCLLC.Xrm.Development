using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Context
{
    using Container;
    using Diagnostics;

    public class LocalPluginContextFactory : ILocalPluginContextFactory
    {      
        public ILocalPluginContext<E> CreateLocalPluginContext<E>(IPluginExecutionContext pluginExecutionContext, IContainer container, IServiceProvider serviceProvider, IDiagnosticService diagnosticService) where E : Entity 
        {
            return new LocalPluginContext<E>(container, serviceProvider, pluginExecutionContext, diagnosticService);
        }     

    }
}
