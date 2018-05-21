using System;
using CCLLC.Xrm.Sdk.Caching;

namespace CCLLC.Xrm.Sdk
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
