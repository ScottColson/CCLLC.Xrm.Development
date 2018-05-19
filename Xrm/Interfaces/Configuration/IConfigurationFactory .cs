using Microsoft.Xrm.Sdk;

namespace CCLCC.Xrm.Sdk
{
    using Caching;
    using Encryption;

    public interface IConfigurationFactory
    {
        IExtensionSettings BuildExtensionSettings(IOrganizationService orgService, IXrmCache cache, IEncryption encryption, string key = null);

        IXmlConfigurationResource BuildConfigurationResources(IOrganizationService orgService, IXrmCache cache);
    }
}
