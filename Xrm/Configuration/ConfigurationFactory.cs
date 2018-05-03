using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLCC.Xrm.Caching;
using CCLCC.Xrm.Encryption;
using Microsoft.Xrm.Sdk;

namespace CCLCC.Xrm.Configuration
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
