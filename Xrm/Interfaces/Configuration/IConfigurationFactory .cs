using Microsoft.Xrm.Sdk;

namespace CCLCC.Xrm.Sdk
{
    using Caching;
    using Encryption;

    public interface IConfigurationFactory
    {
        IExtensionSettings CreateExtensionSettings(IOrganizationService orgService, IXrmCache cache, IEncryption encryption, string key = null);

        IXmlConfigurationResource CreateConfigurationResources(IOrganizationService orgService, IXrmCache cache);
    }
}
