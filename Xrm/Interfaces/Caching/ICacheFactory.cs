using System;

namespace CCLCC.Xrm.Sdk
{
    public interface ICacheFactory
    {
        IXrmCache CreatePluginCache();
        IXrmCache CreateOrganizationCache(Guid organizationId);
    }
}
