using Microsoft.Xrm.Sdk;
using CCLLC.Xrm.Sdk.Configuration;

namespace CCLLC.Xrm.Sdk
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        public IXmlConfigurationResource BuildConfigurationResources(IOrganizationService orgService, IXrmCache cache)
        {
            return new XmlConfigurationResource(orgService, cache);
        }

        public IExtensionSettings BuildExtensionSettings(IOrganizationService orgService, IXrmCache cache, IEncryption encryption, IExtensionSettingsConfig config)
        {
            return new ExtensionSettings(orgService, cache, encryption, config);
        }
    }
}
