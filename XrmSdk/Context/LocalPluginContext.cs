using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using CCLLC.Core;

namespace CCLLC.Xrm.Sdk.Context
{
    using Utilities;

    public class LocalPluginContext : LocalContext, IDisposable, ILocalPluginContext
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IPluginExecutionContext PluginExecutionContext { get { return (IPluginExecutionContext)base.ExecutionContext; } }

        public ePluginStage Stage { get { return (ePluginStage)this.PluginExecutionContext.Stage; } }

        /// <summary>
        /// Returns the first registered 'Pre' image for the pipeline execution
        /// </summary>
        public Entity PreImage 
        {
            get
            {
                if (this.PluginExecutionContext.PreEntityImages.Any())
                {
                    return this.PluginExecutionContext.PreEntityImages[this.PluginExecutionContext.PreEntityImages.FirstOrDefault().Key];
                }
                return null;
            }
            
        } 

        /// <summary>
        /// Returns the first registered 'Post' image for the pipeline execution
        /// </summary>
        public Entity PostImage
        {
            get
            {
                if (this.PluginExecutionContext.PostEntityImages.Any())
                {
                    return this.PluginExecutionContext.PostEntityImages[this.PluginExecutionContext.PostEntityImages.FirstOrDefault().Key];
                }
                return null;
            }
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

        internal LocalPluginContext(IServiceProvider serviceProvider, IIocContainer container, IPluginExecutionContext executionContext)
            : base(executionContext, container)
        {
            this.ServiceProvider = serviceProvider;
        }

        protected override IOrganizationServiceFactory CreateOrganizationServiceFactory()
        {
            return (IOrganizationServiceFactory)this.ServiceProvider.GetService(typeof(IOrganizationServiceFactory));
        }

        protected override ITracingService CreateTracingService()
        {
            return (ITracingService)this.ServiceProvider.GetService(typeof(ITracingService));
        }
    }

}

