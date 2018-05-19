using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Xrm.Sdk.Configuration
{
    public class DefaultExtensionSettingsConfig : IExtensionSettingsConfig
    {
        public int DefaultTimeout { get; set; }    
        public string EncryptionKey { get; set; }
        public string EntityName { get; set; }
        public string NameColumn { get; set; }
        public string ValueColumn { get; set; }
        public string EncryptionColumn { get; set; }

        public DefaultExtensionSettingsConfig()
        {
            DefaultTimeout = 1800; //30 minutes
            EncryptionKey = "7a5a64brEgaceqenuyegac7era3Ape6aWatrewegeka94waqegayathudrebuc7t";
            EntityName = "cclcc_extensionsettings";
            NameColumn = "cclcc_name";
            ValueColumn = "cclcc_value";
            EncryptionColumn = "cclcc_encryptedflag";
        }
    }
}
