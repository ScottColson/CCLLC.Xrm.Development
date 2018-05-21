using Microsoft.Xrm.Sdk;

namespace CCLLC.Xrm.Sdk
{
    using Caching;
    using Encryption;

    public interface IConfigurationFactory
    {
        IExtensionSettings BuildExtensionSettings(IOrganizationService orgService, IXrmCache cache, IEncryption encryption, IExtensionSettingsConfig config);

        IXmlConfigurationResource BuildConfigurationResources(IOrganizationService orgService, IXrmCache cache);
    }
}
