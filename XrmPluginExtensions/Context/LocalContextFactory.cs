using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions.Context
{
    using Container;
    using Diagnostics;
    using Telemetry;

    public class LocalContextFactory : ILocalContextFactory
    {      
        public ILocalPluginContext<E> CreateLocalPluginContext<E>(IPluginExecutionContext pluginExecutionContext, IContainer container, IServiceProvider serviceProvider, IDiagnosticService diagnosticService) where E : Entity 
        {
            return new LocalPluginContext<E>(container, serviceProvider, pluginExecutionContext, diagnosticService);
        }
    }
}
