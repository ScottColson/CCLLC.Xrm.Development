using System;
using Microsoft.Xrm.Sdk;


namespace CCLCC.XrmPluginExtensions
{
    using CCLCC.XrmPluginExtensions.Container;
    using Telemetry;

    public abstract partial class EventTelemetryPlugin<E> : PluginBase<E> where E : Entity
    {
        public EventTelemetryPlugin(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {           
        }

        public override void RegisterContainerServices(IContainer container)
        {
            base.RegisterContainerServices(container);
            container.RegisterAsSingleInstance<ITelemetryProvider, EventTelemetryProvider>();
        }

       

    }
}
