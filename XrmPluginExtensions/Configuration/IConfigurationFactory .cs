using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.XrmPluginExtensions.Configuration
{
    using Caching;
    using Encryption;

    public interface IConfigurationFactory
    {
        IExtensionSettings CreateExtensionSettings(IOrganizationService orgService, IXrmCache cache, IEncryption encryption, string key = null);

        IXmlConfigurationResource CreateConfigurationResources(IOrganizationService orgService, IXrmCache cache);
    }
}
