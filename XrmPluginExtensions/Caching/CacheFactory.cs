using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Caching
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
