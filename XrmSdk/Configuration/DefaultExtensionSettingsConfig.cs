

namespace CCLLC.Xrm.Sdk.Configuration
{
    /// <summary>
    /// Defines the default configuration for the <see cref="ExtensionSettings"/> implementation based on the CCLLC Extension
    /// Settings entity. These settings are used when the object is created. This object can be overridden to provide support
    /// for an alternate entity setup. When overriding this object the new object should be registered in the plugin IOC container
    /// against the <see cref="IExtensionSettingsConfig"/> interface as part of the <see cref="IEnhancedPlugin.RegisterContainerServices"/>
    /// method.
    /// </summary>
    public class DefaultExtensionSettingsConfig : IExtensionSettingsConfig
    {
        /// <summary>
        /// Defines the setting cache timeout in seconds.
        /// </summary>
        public int DefaultTimeout { get; protected set; }    
        /// <summary>
        /// Defines the ecryption key used when encrypting setting values.
        /// </summary>
        public string EncryptionKey { get; protected set; }
        /// <summary>
        /// Defines the name of the entity that contains extension settings.
        /// </summary>
        public string EntityName { get; protected set; }
        /// <summary>
        /// Defines the name of the column that contains the setting name.
        /// </summary>
        public string NameColumn { get; protected set; }
        /// <summary>
        /// Defines the name of the column that contains the setting value.
        /// </summary>
        public string ValueColumn { get; protected set; }
        /// <summary>
        /// Defines the name of the column that indicates the value is encrypted.
        /// </summary>
        public string EncryptionColumn { get; protected set; }


        public DefaultExtensionSettingsConfig()
        {
            DefaultTimeout = 1800; //30 minutes
            EncryptionKey = "7a5a64brEgaceqenuyegac7era3Ape6aWatrewegeka94waqegayathudrebuc7t";
            EntityName = "ccllc_extensionsettings";
            NameColumn = "ccllc_name";
            ValueColumn = "ccllc_value";
            EncryptionColumn = "ccllc_encryptedflag";
        }
    }
}
