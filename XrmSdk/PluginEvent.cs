using System;
using Microsoft.Xrm.Sdk;

namespace CCLLC.Xrm.Sdk
{
    using Context;
    using Telemetry;

    public class PluginEvent
    {
        private string _id;
        /// <summary>
        /// Identifing name for the handler. Used in logging events.
        /// </summary>
        public string Id
        {
            get { return _id ?? string.Empty; }
            set { _id = value; }
        }

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
        public Action<ILocalPluginContext> PluginAction  { get; set; }
}
}
