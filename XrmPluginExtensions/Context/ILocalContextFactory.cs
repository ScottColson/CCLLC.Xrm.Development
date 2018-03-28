using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions.Context
{
    using Container;
    using Diagnostics;
    using Telemetry;


    public interface ILocalContextFactory 
    {
        ILocalPluginContext<E> CreateLocalPluginContext<E>(IPluginExecutionContext pluginExecutionContext, IContainer container, IServiceProvider serviceProvider, IDiagnosticService diagnosticService) where E : Entity;
    }
}
