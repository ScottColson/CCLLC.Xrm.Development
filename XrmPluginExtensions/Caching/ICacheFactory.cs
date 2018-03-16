using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.XrmPluginExtensions.Caching
{
    public interface ICacheFactory
    {
        IXrmCache CreatePluginCache();
        IXrmCache CreateOrganizationCache(Guid organizationId);
    }
}
