using System;

namespace CCLCC.Xrm.Caching
{
    public interface ICacheFactory
    {
        IXrmCache CreatePluginCache();
        IXrmCache CreateOrganizationCache(Guid organizationId);
    }
}
