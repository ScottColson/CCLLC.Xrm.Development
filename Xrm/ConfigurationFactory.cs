using Microsoft.Xrm.Sdk;
using CCLCC.Xrm.Sdk.Configuration;

namespace CCLCC.Xrm.Sdk
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        public IXmlConfigurationResource BuildConfigurationResources(IOrganizationService orgService, IXrmCache cache)
        {
            return new XmlConfigurationResource(orgService, cache);
        }

        public IExtensionSettings BuildExtensionSettings(IOrganizationService orgService, IXrmCache cache, IEncryption encryption, string key = null)
        {
            return new ExtensionSettings(orgService, cache, encryption, key);
        }
    }
}
