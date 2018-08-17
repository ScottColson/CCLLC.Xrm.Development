
namespace CCLLC.Xrm.Sdk
{
    /// <summary>
    /// Defines settings passed in during the creation of <see cref="CCLLC.Xrm.Sdk.Configuration.ExtensionSettings"/>.
    /// </summary>
    public interface IExtensionSettingsConfig
    {
        /// <summary>
        /// Defines the setting cache timeout in seconds.
        /// </summary>
        int DefaultTimeout { get;  }
        /// <summary>
        /// Defines the ecryption key used when encrypting setting values.
        /// </summary>
        string EncryptionKey { get;  }
        /// <summary>
        /// Defines the name of the entity that contains extension settings.
        /// </summary>
        string EntityName { get;  }
        /// <summary>
        /// Defines the name of the column that contains the setting name.
        /// </summary>
        string NameColumn { get;  }
        /// <summary>
        /// Defines the name of the column that contains the setting value.
        /// </summary>
        string ValueColumn { get;  }
        /// <summary>
        /// Defines the name of the column that indicates the value is encrypted.
        /// </summary>
        string EncryptionColumn { get; }
    }
}
