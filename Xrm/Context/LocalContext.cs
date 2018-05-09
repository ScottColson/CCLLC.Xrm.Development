using System;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Context
{
    using Caching;
    using Configuration;
    using Encryption;
    

    public abstract class LocalContext<E> : ILocalContext<E> where E : Entity
    {
        public Func<bool> OnConfigureTelemetrySink
        {
            get { return this.TelemetryClient.TelemetrySink.OnConfigure; }
            set { this.TelemetryClient.TelemetrySink.OnConfigure = value; }
        }

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

        public IComponentTelemetryClient TelemetryClient { get; private set; }

        private ITelemetryFactory telemetryFactory;
        public ITelemetryFactory TelemetryFactory
        {
            get
            {
                if (telemetryFactory == null)
                {
                    telemetryFactory = this.Container.Resolve<ITelemetryFactory>();
                }
                return telemetryFactory;
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
                    organizationCache = factory.CreateOrganizationCache(this.ExecutionContext.OrganizationId);
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
                    pluginCache = factory.CreatePluginCache();
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
                    extensionSettings = factory.CreateExtensionSettings(this.ElevatedOrganizationService, this.OrganizationCache, encryption);
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
                    xmlConfigurationResources = factory.CreateConfigurationResources(this.ElevatedOrganizationService, this.OrganizationCache);
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

        public LocalContext(IExecutionContext executionContext, IIocContainer container, IComponentTelemetryClient telemetryClient)
        {
            if (container == null) throw new ArgumentNullException("container");
            this.Container = container;
           
            if (executionContext == null) throw new ArgumentNullException("executionContext");
            this.ExecutionContext = executionContext;

            if (telemetryClient == null) throw new ArgumentNullException("telemetryClient");
            this.TelemetryClient = telemetryClient;
        }

        public virtual void Dispose()
        {
            if (this.TelemetryClient != null)
            {
                if (this.TelemetryClient.TelemetrySink != null)
                {
                    this.TelemetryClient.TelemetrySink.OnConfigure = null;
                }
                this.TelemetryClient.Dispose();
            }   
        }
        

        protected E GetEntityAsType(Entity entity)
        {
            if (typeof(E) == entity.GetType())
                return entity as E;
            else
                return entity.ToEntity<E>();
        }

        protected abstract IOrganizationServiceFactory CreateOrganizationServiceFactory();

    }
}
