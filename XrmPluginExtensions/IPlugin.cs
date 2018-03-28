using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xrm.Sdk;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions
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

        ITelemetryProvider TelemetryProvider { get; }

        Dictionary<string,string> TelemetryServiceFactorySettings { get; }

        void RegisterMessageHandler(string entityName, string messageName, ePluginStage stage, Action<ILocalContext<E>> handler);

        void RegisterContainerServices(IContainer container);

        void ConfigureTelemetryProvider(ITelemetryProvider telemetryProvider);
    }
}
