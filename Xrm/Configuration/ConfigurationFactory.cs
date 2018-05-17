using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.Xrm.Sdk.Configuration
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
