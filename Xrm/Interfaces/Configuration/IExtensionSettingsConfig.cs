using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Xrm.Sdk
{
    public interface IExtensionSettingsConfig
    {
        int DefaultTimeout { get; set; }
        string EncryptionKey { get; set; }

        string EntityName { get; set; }
        string NameColumn { get; set; }
        string ValueColumn { get; set; }
        string EncryptionColumn { get; set; }
    }
}
