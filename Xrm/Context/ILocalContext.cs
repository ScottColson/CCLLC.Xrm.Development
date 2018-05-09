using System;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Context
{
    using Caching;
    using Configuration;   

    public interface ILocalContext<E> : IDisposable where E : Entity
    {
        Action OnConfigureTelemetrySink { get; set; }

        IExecutionContext ExecutionContext { get; }
        IOrganizationServiceFactory OrganizationServiceFactory { get; }
        IOrganizationService OrganizationService { get; }

        IOrganizationService ElevatedOrganizationService { get; }
        IIocContainer Container { get; }

        IApplicationTelemetryClient TelemetryClient { get; }

        ITelemetryFactory TelemetryFactory { get; }

        int Depth { get; }
        string MessageName { get; }
        IXrmCache OrganizationCache { get; }

        IXrmCache PluginCache { get; }

        IExtensionSettings ExtensionSettings { get; }
        IXmlConfigurationResource XmlConfigurationResources { get; }

        E TargetEntity { get; }

        EntityReference TargetReference { get; }

    }
}
