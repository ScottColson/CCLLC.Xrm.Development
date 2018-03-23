using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions
{
    using System;
    using Telemetry;

    public abstract class Plugin<E> : PluginBase<E, ITelemetryService> where E : Entity
    {
        public Plugin(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig) { }
      
        public Plugin() : base() { }

    }
}
