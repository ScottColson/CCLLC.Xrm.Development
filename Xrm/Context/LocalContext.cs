using System;
using Microsoft.Xrm.Sdk;
using CCLLC.Core;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk.Context
{
    using Caching;
    using Configuration;
    using Encryption;


    public abstract class LocalContext<E> : ILocalContext<E> where E : Entity
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


        public E TargetEntity
        {
            get
            {
                if (this.ExecutionContext.InputParameters.Contains("Target"))
                {
                    var entity = this.ExecutionContext.InputParameters["Target"] as Entity;
                    return GetEntityAsType(entity);
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
                if (this.ExecutionContext.InputParameters.Contains("Target"))
                    return this.ExecutionContext.InputParameters["Target"] as EntityReference;
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

        public LocalContext(IExecutionContext executionContext, IIocContainer container)
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

        public virtual void Dispose()
        {            
        }
        

        protected E GetEntityAsType(Entity entity)
        {
            if (typeof(E) == entity.GetType())
                return entity as E;
            else
                return entity.ToEntity<E>();
        }

        protected abstract IOrganizationServiceFactory CreateOrganizationServiceFactory();
        protected abstract ITracingService CreateTracingService();
        
        public virtual void Trace(string message, params object[] args)
        {
            this.Trace(eMessageType.Information, message, args);
        }

        public virtual void Trace(eMessageType type, string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var msg = type.ToString() + ": " + message;
                this.TracingService.Trace(msg, args);               
            }

        
        }
    }
}
