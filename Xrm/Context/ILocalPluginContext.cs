using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.Xrm.Context
{

    public interface ILocalPluginContext<E> : ILocalContext<E> where E : Entity 
    {

        IServiceProvider ServiceProvider { get; }      
        ePluginStage Stage { get; }
        IPluginExecutionContext PluginExecutionContext { get; }
        E PreImage { get; }
        E PostImage { get; }
        E PreMergedTarget { get; }
    }
}
