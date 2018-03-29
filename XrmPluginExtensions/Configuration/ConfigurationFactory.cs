using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLCC.XrmBase.Caching;
using CCLCC.XrmBase.Encryption;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Configuration
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
