using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D365.XrmPluginExtensions.Caching;
using D365.XrmPluginExtensions.Encryption;
using Microsoft.Xrm.Sdk;

namespace D365.XrmPluginExtensions.Configuration
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        public IXmlConfigurationResource CreateConfigurationResources(IOrganizationService orgService, IXrmCache cache)
        {
            throw new NotImplementedException();
        }

        public IExtensionSettings CreateExtensionSettings(IOrganizationService orgService, IXrmCache cache, IEncryption encryption, string key = null)
        {
            throw new NotImplementedException();
        }
    }
}
