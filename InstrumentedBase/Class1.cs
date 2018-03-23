using System;
using Microsoft.Xrm.Sdk;


namespace CCLCC.XrmPluginExtensions
{
    using Telemetry;

    public abstract partial class EventTelemetryPlugin<E> : PluginBase<E,IEventTelemetryService> where E : Entity
    {
        public EventTelemetryPlugin(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {           
        }

        public override IServiceProvider<IEventTelemetryService> DecorateServiceProvider(IServiceProvider provider)
        {            
            return new EventTelemetryServiceProvider<IEventTelemetryService>(base.DecorateServiceProvider(provider));
        }

    }
}
