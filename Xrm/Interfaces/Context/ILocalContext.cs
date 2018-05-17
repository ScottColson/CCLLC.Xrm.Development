using System;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Sdk
{
    using Caching;
    using Configuration;   

    public interface ILocalContext<E> : IDisposable where E : Entity
    {
        //Func<bool> OnConfigureTelemetrySink { get; set; }

        void SetAlternateDataKey(string name, string value);
       
        void Trace(string message, params object[] args);
        void Trace(Telemetry.SeverityLevel level, string message, params object[] args);

        IExecutionContext ExecutionContext { get; }
        IOrganizationServiceFactory OrganizationServiceFactory { get; }
        IOrganizationService OrganizationService { get; }

        IOrganizationService ElevatedOrganizationService { get; }
        IIocContainer Container { get; }

        ITracingService TracingService { get;  }
        IComponentTelemetryClient TelemetryClient { get; }

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
