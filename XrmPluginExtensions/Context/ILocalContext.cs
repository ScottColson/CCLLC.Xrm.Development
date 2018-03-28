
using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Context
{
    using Caching;
    using Configuration;
    using Container;
    using Diagnostics;
    using Telemetry;


    public interface ILocalContext<E> : IDisposable where E : Entity
    {
        IServiceProvider ServiceProvider { get; }
        IContainer Container { get; }
        IOrganizationServiceFactory OrganizationServiceFactory { get; }
        IOrganizationService OrganizationService { get; }

        IOrganizationService ElevatedOrganizationService { get; }

        IPluginExecutionContext PluginExecutionContext { get;  }
        IDiagnosticService DiagnosticService { get; }
        ePluginStage Stage { get; }
        int Depth { get; }
        string MessageName { get; }
        IXrmCache OrganizationCache { get; }

        IXrmCache PluginCache { get; }

        IExtensionSettings ExtensionSettings { get; }
        IXmlConfigurationResource XmlConfigurationResources { get; }
    }
}
