using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase
{
    using Container;
    using Context;
    using Telemetry;

    public interface IPlugin<E> : Microsoft.Xrm.Sdk.IPlugin where E : Entity
    {
        IContainer Container { get; }
        IReadOnlyList<PluginEvent<E>> PluginEventHandlers { get; }
        string UnsecureConfig { get; }
        string SecureConfig { get; }

        void RegisterMessageHandler(string entityName, string messageName, ePluginStage stage, Action<ILocalContext<E>> handler);

        void RegisterContainerServices(IContainer container);

        ConfigureTelemtryProvider GetConfigureTelemetryProviderCallback(ILocalContext<E> localContext);
    }
}
