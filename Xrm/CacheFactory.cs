using System;
using CCLCC.Xrm.Sdk.Caching;

namespace CCLCC.Xrm.Sdk
{
    public class CacheFactory : ICacheFactory
    {
        public IXrmCache BuildOrganizationCache(Guid organizationId)
        {
            return new XrmOrganizationCache(organizationId);
        }

        public IXrmCache BuildPluginCache()
        {
            return XrmPluginCache.Instance;
        }
    }
}
