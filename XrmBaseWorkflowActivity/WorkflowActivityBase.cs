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
    using Diagnostics;
    using Telemetry;

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
            container.RegisterAsSingleInstance<ITelemetryProvider, TracingTelemetryProvider>();
            container.RegisterAsSingleInstance<ICacheFactory, CacheFactory>();
            container.RegisterAsSingleInstance<IConfigurationFactory, ConfigurationFactory>();
            container.RegisterAsSingleInstance<IDiagnosticServiceFactory, DiagnosticServiceFactory>();
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
            var diagnosticServiceFactory = Container.Resolve<IDiagnosticServiceFactory>();

            var executionContext = codeActivityContext.GetExtension<IWorkflowContext>();

            using (var diagnosticService = diagnosticServiceFactory.CreateDiagnosticService(this.GetType().ToString(), executionContext, tracingService, telemetryProvider))
            {
                try
                {
                    var localContextFactory = Container.Resolve<ILocalWorkflowActivityContextFactory>();

                    using (var localContext = localContextFactory.CreateLocalWorkflowActivityContext<E>(executionContext, this.Container, codeActivityContext, diagnosticService))
                    {
                        localContext.SetConfigureTelemetryProviderCallback(GetConfigureTelemetryProviderCallback(localContext));

                        ExecuteInternal(localContext);
                    }
                }
                catch (InvalidWorkflowException ex)
                {
                    diagnosticService.TraceWorkflowException(ex);
                    throw;
                }
                catch (InvalidPluginExecutionException ex)
                {
                    diagnosticService.TracePluginException(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    diagnosticService.TraceGeneralException(ex);
                    throw;
                }
            }
        }
    }
}
