using System;
using Microsoft.Xrm.Sdk;
using CCLLC.Core;

namespace CCLLC.Xrm.Sdk
{   
    public interface ILocalContext : IDisposable 
    {        
        void Trace(string message, params object[] args);
        void Trace(eMessageType type, string message, params object[] args);

        IExecutionContext ExecutionContext { get; }
        IOrganizationServiceFactory OrganizationServiceFactory { get; }
        IOrganizationService OrganizationService { get; }

        IOrganizationService ElevatedOrganizationService { get; }
        IIocContainer Container { get; }

        ITracingService TracingService { get;  }     
        
        int Depth { get; }
        string MessageName { get; }
        IXrmCache OrganizationCache { get; }

        IXrmCache PluginCache { get; }

        IExtensionSettings ExtensionSettings { get; }
        IXmlConfigurationResource XmlConfigurationResources { get; }

        Entity TargetEntity { get; }

        EntityReference TargetReference { get; }

        IPluginWebRequestFactory WebRequestFactory { get; }

        IPluginWebRequest CreateWebRequest(Uri address, string dependencyName = null);
    }
}
