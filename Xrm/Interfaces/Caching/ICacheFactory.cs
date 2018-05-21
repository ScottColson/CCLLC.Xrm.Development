using System;

namespace CCLLC.Xrm.Sdk
{
    public interface ICacheFactory
    {
        IXrmCache BuildPluginCache();
        IXrmCache BuildOrganizationCache(Guid organizationId);
    }
}
