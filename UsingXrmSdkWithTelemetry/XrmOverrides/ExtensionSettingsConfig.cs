namespace UsingXrmSdkWithTelemetry.XrmOverrides
{
    /// <summary>
    /// Example Override for the default settings for <see cref="CCLLC.Xrm.Sdk.Configuration.ExtensionSettings"/> to 
    /// use entity "new_extensionsettings" with alternate encryption key. Note that this could also be
    /// accomplished by building a new object with the <see cref="CCLLC.Xrm.Sdk.IExtensionSettingsConfig"/>
    /// interface.
    /// </summary>
    public class ExtensionSettingsConfig : CCLLC.Xrm.Sdk.Configuration.DefaultExtensionSettingsConfig
    {
        public ExtensionSettingsConfig() : base()
        {
            DefaultTimeout = 1800; //30 minutes
            EncryptionKey = "7a5a64brEgacnuieygsc7era3Ape6aWatrewegeka94waaasggeyathudrebuc7t";
            EntityName = "new_extensionsettings";
            NameColumn = "new_name";
            ValueColumn = "new_value";
            EncryptionColumn = "new_encryptedflag";
        }
    }
}
