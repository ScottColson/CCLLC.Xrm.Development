using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions.Context
{
    using Caching;
    using Configuration;
    using Diagnostics;
    using Encryption;
    using Telemetry;
    using Utilities;

    public class LocalPluginContext<E,T> : ILocalPluginContext<E,T> where E : Entity where T : ITelemetryService
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IPluginExecutionContext PluginExecutionContext { get; private set; }
        public IDiagnosticService<T> DiagnosticService { get; private set; }

        public ePluginStage Stage { get { return (ePluginStage)this.PluginExecutionContext.Stage; } }
        public int Depth { get { return this.PluginExecutionContext.Depth; } }
        public string MessageName { get { return this.PluginExecutionContext.MessageName; } }

        private IOrganizationServiceFactory organizationServiceFactory;
        public IOrganizationServiceFactory OrganizationServiceFactory
        {
            get
            {
                if (organizationServiceFactory == null)
                {
                    organizationServiceFactory = (IOrganizationServiceFactory)this.ServiceProvider.GetService(typeof(IOrganizationServiceFactory));
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
                    organizationService = this.OrganizationServiceFactory.CreateOrganizationService(this.PluginExecutionContext.UserId);
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

        private Microsoft.Xrm.Sdk.Client.OrganizationServiceContext _crmContext = null;
        /// <summary>
        /// CrmContext to use with LINQ etc.
        /// </summary>
        public Microsoft.Xrm.Sdk.Client.OrganizationServiceContext CrmContext
        {
            get
            {
                if (_crmContext == null)
                {
                    _crmContext = new Microsoft.Xrm.Sdk.Client.OrganizationServiceContext(this.OrganizationService);
                }
                return _crmContext;
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
                    var factory = (IConfigurationFactory)this.ServiceProvider.GetService(typeof(IConfigurationFactory));
                    var encryption = (IRijndaelEncryption)this.ServiceProvider.GetService(typeof(IRijndaelEncryption));
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
                    var factory = (IConfigurationFactory)ServiceProvider.GetService(typeof(IConfigurationFactory));
                    xmlConfigurationResources = factory.CreateConfigurationResources(this.ElevatedOrganizationService, this.OrganizationCache);
                }

                return xmlConfigurationResources;
            }
        }




       



        public LocalPluginContext(IServiceProvider serviceProvider, IPluginExecutionContext executionContext, IDiagnosticService<T> diagnosticService)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");
            this.ServiceProvider = serviceProvider;

            if (executionContext == null) throw new ArgumentNullException("executionContext");
            this.PluginExecutionContext = executionContext;

            if (diagnosticService == null) throw new ArgumentNullException("diagnosticService");
            this.DiagnosticService = diagnosticService;
        }

        public void Dispose()
        {
            if (this._crmContext != null)
                this._crmContext.Dispose();
        }

        /// <summary>
        /// Returns the first registered 'Pre' image for the pipeline execution
        /// </summary>
        public E PreImage
        {
            get
            {
                if (this.PluginExecutionContext.PreEntityImages.Any())
                {
                    return GetEntityAsType(this.PluginExecutionContext.PreEntityImages[this.PluginExecutionContext.PreEntityImages.FirstOrDefault().Key]);
                }
                return null;
            }
        }
        /// <summary>
        /// Returns the first registered 'Post' image for the pipeline execution
        /// </summary>
        public E PostImage
        {
            get
            {
                if (this.PluginExecutionContext.PostEntityImages.Any())
                {
                    return GetEntityAsType(this.PluginExecutionContext.PostEntityImages[this.PluginExecutionContext.PostEntityImages.FirstOrDefault().Key]);
                }
                return null;
            }
        }
        /// <summary>
        /// Returns the 'Target' of the message if available
        /// This is an 'Entity' instead of the specified type in order to retain the same instance of the 'Entity' object. This allows for updates to the target in a 'Pre' stage that
        /// will get persisted during the transaction.
        /// </summary>
        public Entity TargetEntity
        {
            get
            {
                if (this.PluginExecutionContext.InputParameters.Contains("Target"))
                    return this.PluginExecutionContext.InputParameters["Target"] as Entity;

                return null;
            }
        }

        /// <summary>
        /// Returns the 'Target' of the message as an EntityReference if available
        /// </summary>
        public EntityReference TargetEntityReference
        {
            get
            {
                if (this.PluginExecutionContext.InputParameters.Contains("Target"))
                    return this.PluginExecutionContext.InputParameters["Target"] as EntityReference;
                return null;
            }
        }


        private E GetEntityAsType(Entity entity)
        {
            if (typeof(E) == entity.GetType())
                return entity as E;
            else
                return entity.ToEntity<E>();
        }


        private Entity _preMergedTarget = null;
        /// <summary>
        /// Returns an Entity record with all attributes from the current inbound target and any additional attributes
        /// that might exist in the Pre Image entity if provided. PreMergedTarget is cached so future calls
        /// return the same entity object and will not reflect changes made to that Target since initial
        /// request.
        /// </summary>
        public Entity PreMergedTarget
        {
            get
            {
                if (_preMergedTarget == null)
                {
                    _preMergedTarget = new Entity(this.PluginExecutionContext.PrimaryEntityName);
                    _preMergedTarget.Id = this.PluginExecutionContext.PrimaryEntityId;
                    _preMergedTarget.MergeWith(this.TargetEntity);
                    _preMergedTarget.MergeWith(this.PreImage);
                }

                return _preMergedTarget;
            }
        }

        private IXrmCache pluginCache;
        public IXrmCache PluginCache
        {
            get
            {
                if(pluginCache == null)
                {
                    var factory = (ICacheFactory)this.ServiceProvider.GetService(typeof(ICacheFactory));
                    pluginCache = factory.CreatePluginCache();
                }
                return pluginCache;
            }
        }

        IXrmCache organizationCache;
        public IXrmCache OrganizationCache
        {
            get
            {
                if (organizationCache == null)
                {
                    var factory = (ICacheFactory)this.ServiceProvider.GetService(typeof(ICacheFactory));
                    organizationCache = factory.CreateOrganizationCache(this.PluginExecutionContext.OrganizationId);
                }
                return organizationCache;
            }
        }
    }

}

