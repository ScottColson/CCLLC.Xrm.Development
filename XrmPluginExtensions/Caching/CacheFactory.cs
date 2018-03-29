using System;

namespace CCLCC.XrmBase.Caching
{
    public class CacheFactory : ICacheFactory
    {
        public IXrmCache CreateOrganizationCache(Guid organizationId)
        {
            return new XrmOrganizationCache(organizationId);
        }

        public IXrmCache CreatePluginCache()
        {
            return new XrmPluginCache();
        }
    }
}
