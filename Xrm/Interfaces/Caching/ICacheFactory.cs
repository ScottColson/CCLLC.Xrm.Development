using System;

namespace CCLCC.Xrm.Sdk
{
    public interface ICacheFactory
    {
        IXrmCache BuildPluginCache();
        IXrmCache BuildOrganizationCache(Guid organizationId);
    }
}
