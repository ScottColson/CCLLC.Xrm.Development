using System;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Context
{
    using Caching;
    using Configuration;
    using Container;
    using Telemetry;


    public interface ILocalContext<E> : IDisposable where E : Entity
    {
      
        IExecutionContext ExecutionContext {get;}
        IOrganizationServiceFactory OrganizationServiceFactory { get; }
        IOrganizationService OrganizationService { get; }

        IOrganizationService ElevatedOrganizationService { get; }
        IContainer Container { get; }    
        ITelemetryService TelemetryService { get; }
        
        int Depth { get; }
        string MessageName { get; }
        IXrmCache OrganizationCache { get; }

        IXrmCache PluginCache { get; }

        IExtensionSettings ExtensionSettings { get; }
        IXmlConfigurationResource XmlConfigurationResources { get; }

        E TargetEntity { get; }

        EntityReference TargetReference { get; }

        void SetConfigureTelemetryProviderCallback(ConfigureTelemtryProvider callback);

}
}
