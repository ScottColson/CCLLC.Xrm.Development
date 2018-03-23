using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xrm.Sdk;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions
{
    using Context;
    using Telemetry;

    public interface IPlugin<E,T> : Microsoft.Xrm.Sdk.IPlugin where E : Entity where T : ITelemetryService
    {
        IReadOnlyList<PluginEvent<E,T>> PluginEventHandlers { get; }
        string UnsecureConfig { get; }
        string SecureConfig { get; }

        ITelemetryProvider<T> TelemetryProvider { get; }

        Dictionary<string,string> TelemetryServiceFactorySettings { get; }

        void RegisterMessageHandler(string entityName, string messageName, ePluginStage stage, Action<ILocalContext<E, T>> handler);

        IServiceProvider<T> DecorateServiceProvider(IServiceProvider provider);

        
    }
}
