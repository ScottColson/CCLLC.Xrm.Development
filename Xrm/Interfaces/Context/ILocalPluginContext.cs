using System;
using Microsoft.Xrm.Sdk;

namespace CCLLC.Xrm.Sdk
{

    public interface ILocalPluginContext : ILocalContext
    {

        IServiceProvider ServiceProvider { get; }      
        ePluginStage Stage { get; }
        IPluginExecutionContext PluginExecutionContext { get; }
        Entity PreImage { get; }
        Entity PostImage { get; }
        Entity PreMergedTarget { get; }
    }
}
