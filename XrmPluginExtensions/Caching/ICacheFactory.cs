using System;

namespace CCLCC.XrmBase.Caching
{
    public interface ICacheFactory
    {
        IXrmCache CreatePluginCache();
        IXrmCache CreateOrganizationCache(Guid organizationId);
    }
}
