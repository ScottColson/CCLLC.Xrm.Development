using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions
{
    using Context;
    using Telemetry;

    public class PluginEvent<E,T> where E : Entity where T : ITelemetryService
    {
        /// <summary>
        /// Execution pipeline stage that the plugin should be registered against.
        /// </summary>
        public ePluginStage Stage { get; set; }
        /// <summary>
        /// Logical name of the entity that the plugin should be registered against. Leave 'null' to register against all entities.
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// Name of the message that the plugin should be triggered off of.
        /// </summary>
        public string MessageName { get; set; }
        /// <summary>
        /// Method that should be executed when the conditions of the Plugin Event have been met.
        /// </summary>
        public Action<ILocalPluginContext<E,T>> PluginAction  { get; set; }
}
}
