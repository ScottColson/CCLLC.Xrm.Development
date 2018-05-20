using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Sdk
{

    using Context;
  

    public interface IPlugin<E> : Microsoft.Xrm.Sdk.IPlugin where E : Entity
    {
        /// <summary>
        /// IOC container used to instantiate various objects required during the execution of the plugin.
        /// </summary>
        IIocContainer Container { get; }
       
        IReadOnlyList<PluginEvent<E>> PluginEventHandlers { get; }
        string UnsecureConfig { get; }
        string SecureConfig { get; }

        void RegisterEventHandler(string entityName, string messageName, ePluginStage stage, Action<ILocalContext<E>> handler);

        void RegisterContainerServices();

       
    }
}
