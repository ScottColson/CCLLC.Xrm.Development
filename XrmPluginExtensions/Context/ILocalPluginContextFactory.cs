using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Context
{
    using Container;
    using Diagnostics;   

    public interface ILocalPluginContextFactory 
    {
        ILocalPluginContext<E> CreateLocalPluginContext<E>(IPluginExecutionContext executionContext, IContainer container, IServiceProvider serviceProvider, IDiagnosticService diagnosticService) where E : Entity;

    }
}
