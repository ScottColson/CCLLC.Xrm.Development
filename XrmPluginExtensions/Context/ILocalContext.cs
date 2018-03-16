
using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.XrmPluginExtensions.Context
{
    using Caching;
    using Configuration;
    using Diagnostics;


    public interface ILocalContext
    {
        IServiceProvider ServiceProvider { get; }
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
