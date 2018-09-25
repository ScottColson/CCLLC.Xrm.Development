using System;
using Microsoft.Xrm.Sdk;
using CCLLC.Core;

namespace CCLLC.Xrm.Sdk.Context
{  
    public abstract class LocalContext : ILocalContext 
    {
        public IIocContainer Container { get; private set; }
        public IExecutionContext ExecutionContext { get; private set; }

        private IOrganizationServiceFactory organizationServiceFactory;
        public IOrganizationServiceFactory OrganizationServiceFactory
        {
            get
            {
                if (organizationServiceFactory == null)
                {
                    organizationServiceFactory = CreateOrganizationServiceFactory();
                }
                return organizationServiceFactory;
            }
        }


        private IOrganizationService organizationService;
        public IOrganizationService OrganizationService
        {
            get
            {
                if (organizationService == null)
                {
                    organizationService = this.OrganizationServiceFactory.CreateOrganizationService(this.ExecutionContext.UserId);
                }

                return organizationService;
            }
        }

        private IOrganizationService elevatedOrganizationService = null;
        /// <summary>
        /// Access to an organization service that runs with elevated access credentials under 
        /// the SYSTEM user identity.
        /// </summary>
        public IOrganizationService ElevatedOrganizationService
        {
            get
            {
                if (elevatedOrganizationService == null)
                {
                    elevatedOrganizationService = this.OrganizationServiceFactory.CreateOrganizationService(null);
                }

                return elevatedOrganizationService;
            }
        }

        public int Depth { get { return this.ExecutionContext.Depth; } }

        public string MessageName { get { return this.ExecutionContext.MessageName; } }

        IXrmCache organizationCache;
        public IXrmCache OrganizationCache
        {
            get
            {
                if (organizationCache == null)
                {
                    var factory = Container.Resolve<ICacheFactory>();
                    organizationCache = factory.BuildOrganizationCache(this.ExecutionContext.OrganizationId);
                }
                return organizationCache;
            }
        }

        private IXrmCache pluginCache;
        public IXrmCache PluginCache
        {
            get
            {
                if (pluginCache == null)
                {
                    var factory = Container.Resolve<ICacheFactory>();
                    pluginCache = factory.BuildPluginCache();
                }
                return pluginCache;
            }
        }

        private IExtensionSettings extensionSettings = null;
        /// <summary>
        /// Access to name/value settings stored in the Extension Settings entity.
        /// </summary>
        public IExtensionSettings ExtensionSettings
        {
            get
            {
                if (extensionSettings == null)
                {
                    var factory = Container.Resolve<IConfigurationFactory>();
                    var encryption = Container.Resolve<IRijndaelEncryption>();
                    var config = Container.Resolve<IExtensionSettingsConfig>();
                    extensionSettings = factory.BuildExtensionSettings(this.ElevatedOrganizationService, this.OrganizationCache, encryption, config);
                }

                return extensionSettings;
            }
        }

        private IXmlConfigurationResource xmlConfigurationResources = null;
        /// <summary>
        /// Access to configuration resources stored in CRM Xml Data Web Resources.
        /// </summary>
        public IXmlConfigurationResource XmlConfigurationResources
        {
            get
            {
                if (xmlConfigurationResources == null)
                {
                    var factory = Container.Resolve<IConfigurationFactory>();
                    xmlConfigurationResources = factory.BuildConfigurationResources(this.ElevatedOrganizationService, this.OrganizationCache);
                }

                return xmlConfigurationResources;
            }
        }


        public Entity TargetEntity
        {
            get
            {
                if (this.ExecutionContext.InputParameters.Contains("Target"))
                {
                    return this.ExecutionContext.InputParameters["Target"] as Entity;                    
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the 'Target' of the message as an EntityReference if available
        /// </summary>
        public EntityReference TargetReference
        {
            get
            {
                if (this.TargetEntity != null)
                {
                    return this.TargetEntity.ToEntityReference();
                }                   
                return null;
            }
        }

        private ITracingService tracingService;
        public ITracingService TracingService
        {
            get
            {
                if (tracingService == null) { tracingService = CreateTracingService(); }
                return tracingService;
            }
        }

        private IPluginWebRequestFactory _webRequestFactory;
        public IPluginWebRequestFactory WebRequestFactory
        {
            get
            {
                if (_webRequestFactory == null)
                {
                    _webRequestFactory = this.Container.Resolve<IPluginWebRequestFactory>();
                }
                return _webRequestFactory;
            }
        }

        protected internal LocalContext(IExecutionContext executionContext, IIocContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            this.Container = container;
           
            if (executionContext == null) throw new ArgumentNullException("executionContext");
            this.ExecutionContext = executionContext;           
        }

        public virtual IPluginWebRequest CreateWebRequest(Uri address, string dependencyName = null)
        {
            return this.WebRequestFactory.BuildPluginWebRequest(address, dependencyName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }     

        protected virtual void Dispose(bool dispossing)
        {
            if (dispossing)
            {
                this.Container = null;
                this.elevatedOrganizationService = null;
                this.ExecutionContext = null;
                this.extensionSettings = null;
                this.organizationCache = null;
                this.organizationService = null;
                this.organizationServiceFactory = null;
                this.pluginCache = null;
                this.tracingService = null;
                this.xmlConfigurationResources = null;
                this._webRequestFactory = null;
            }          
        }

        protected abstract IOrganizationServiceFactory CreateOrganizationServiceFactory();
        protected abstract ITracingService CreateTracingService();
        
        /// <summary>
        /// Writes a message to the pluign trace log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public virtual void Trace(string message, params object[] args)
        {
            this.Trace(eMessageType.Information, message, args);
        }

        /// <summary>
        /// Writes a message to the plugin trace log.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public virtual void Trace(eMessageType type, string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var msg = type.ToString() + ": " + message;
                this.TracingService.Trace(msg, args);               
            }        
        }

        /// <summary>
        /// Writes an exception entry to the plugin trace log.
        /// </summary>
        /// <param name="ex"></param>
        public virtual void TrackException(Exception ex)
        {
            this.Trace(eMessageType.Error, "Exception: {0}", ex.Message);
        }

        /// <summary>
        /// Writes an event to the plugin trace log.
        /// </summary>
        /// <param name="name"></param>
        public virtual void TrackEvent(string name)
        {
            this.Trace(eMessageType.Information, "Event: {0}", name);
        }
    }
}
