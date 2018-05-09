using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Context
{
    using Utilities;

    public class LocalPluginContext<E> : LocalContext<E>, IDisposable, ILocalPluginContext<E> where E : Entity
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IPluginExecutionContext PluginExecutionContext { get { return (IPluginExecutionContext)base.ExecutionContext; } }

        public ePluginStage Stage { get { return (ePluginStage)this.PluginExecutionContext.Stage; } }

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

        private Entity _preMergedTarget = null;
        /// <summary>
        /// Returns an Entity record with all attributes from the current inbound target and any additional attributes
        /// that might exist in the Pre Image entity if provided. PreMergedTarget is cached so future calls
        /// return the same entity object and will not reflect changes made to that Target since initial
        /// request.
        /// </summary>
        public E PreMergedTarget
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

                return GetEntityAsType(_preMergedTarget);
            }
        }

        internal LocalPluginContext(IServiceProvider serviceProvider, IIocContainer container, IPluginExecutionContext executionContext, IComponentTelemetryClient telemetryClient)
            : base(executionContext, container, telemetryClient)
        {
            this.ServiceProvider = serviceProvider;
        }

        protected override IOrganizationServiceFactory CreateOrganizationServiceFactory()
        {
            return (IOrganizationServiceFactory)this.ServiceProvider.GetService(typeof(IOrganizationServiceFactory));
        }


    }

}

