using System;
using System.Activities;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace CCLCC.XrmBase
{
    using Caching;
    using Configuration;
    using Container;
    using Context;
    using Encryption;
    using Telemetry;
    using Telemetry.Providers;

    public abstract class WorkflowActivityBase<E> : CodeActivity, IWorkflowActivity<E> where E : Entity
    {
        private IContainer container;
        public IContainer Container
        {
            get
            {
                if (container == null)
                {
                    container = new Container.Container();
                    RegisterContainerServices(container);
                }
                return container;
            }
        }

        public virtual void RegisterContainerServices(IContainer container)
        {
            container.Register<ITelemetryProvider, DefaultTelemetryProvider>();
            container.RegisterAsSingleInstance<ICacheFactory, CacheFactory>();
            container.RegisterAsSingleInstance<IConfigurationFactory, ConfigurationFactory>();
            container.RegisterAsSingleInstance<ILocalWorkflowActivityContextFactory, LocalWorkflowActivityContextFactory>();
            container.RegisterAsSingleInstance<IRijndaelEncryption, RijndaelEncryption>();
        }

        public virtual ConfigureTelemtryProvider GetConfigureTelemetryProviderCallback(ILocalContext<E> localContext)
        {
            return null;
        }

        protected abstract void ExecuteInternal(ILocalWorkflowActivityContext<E> context);

        protected override void Execute(CodeActivityContext codeActivityContext)
        {
            if (codeActivityContext == null)
                throw new ArgumentNullException("codeActivityContext");
            
            var tracingService = codeActivityContext.GetExtension<ITracingService>();
            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));

            var telemetryProvider = Container.Resolve<ITelemetryProvider>();
            
            var executionContext = codeActivityContext.GetExtension<IWorkflowContext>();

            using (var telemetryService = telemetryProvider.CreateTelemetryService(this.GetType().ToString(), tracingService, telemetryProvider, executionContext))
            {
                try
                {
                    var localContextFactory = Container.Resolve<ILocalWorkflowActivityContextFactory>();

                    using (var localContext = localContextFactory.CreateLocalWorkflowActivityContext<E>(executionContext, Container, codeActivityContext, telemetryService))
                    {
                        localContext.SetConfigureTelemetryProviderCallback(GetConfigureTelemetryProviderCallback(localContext));

                        ExecuteInternal(localContext);
                    }
                }          
                catch (Exception ex)
                {
                    telemetryService.TraceException(ex);
                    throw;
                }
            }
        }
    }
}
