using System;
using System.Activities;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Workflow
{
    using Caching;
    using Configuration;
    using Context;
    using Encryption;
    using System.Collections.Generic;

    public abstract class WorkflowActivityBase<E> : CodeActivity, IWorkflowActivity<E> where E : Entity
    {
        private IIocContainer container;
        public IIocContainer Container
        {
            get
            {
                if (container == null)
                {
                    container = new IocContainer();
                    RegisterContainerServices(container);
                }
                return container;
            }
        }

        private ITelemetrySink telemetrySink;
        public ITelemetrySink TelemetrySink
        {
            get
            {
                if (telemetrySink == null)
                {
                    telemetrySink = Container.Resolve<ITelemetrySink>();
                }
                return telemetrySink;
            }
        }      

        public virtual void RegisterContainerServices(IIocContainer container)
        {
            container.Register<ITelemetrySink, CCLCC.Telemetry.Sink.TelemetrySink>();
            container.Register<ITelemetryFactory, CCLCC.Telemetry.TelemetryFactory>();
            container.Register<IApplicationTelemetryClientFactory, IApplicationTelemetryClientFactory>();
            container.Register<ICacheFactory, CacheFactory>();
            container.Register<IConfigurationFactory, ConfigurationFactory>();
            container.Register<ILocalWorkflowActivityContextFactory, LocalWorkflowActivityContextFactory>();
            container.Register<IRijndaelEncryption, RijndaelEncryption>();
        }
       
        public abstract void ExecuteInternal(ILocalWorkflowActivityContext<E> context);

        protected override void Execute(CodeActivityContext codeActivityContext)
        {
            if (codeActivityContext == null)
                throw new ArgumentNullException("codeActivityContext");
            
            var tracingService = codeActivityContext.GetExtension<ITracingService>();
            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));

            var telemetryFactory = Container.Resolve<ITelemetryFactory>();
            var telemetryClientFactory = Container.Resolve<IApplicationTelemetryClientFactory>();

            var executionContext = codeActivityContext.GetExtension<IWorkflowContext>();

            using (var telemetryClient = telemetryClientFactory.BuildClient(
                 this.GetType().ToString(),
                 this.TelemetrySink,
                 new Dictionary<string, string>{
                    { "crm-correlationid", executionContext.CorrelationId.ToString() },
                    { "crm-depth", executionContext.Depth.ToString() },
                    { "crm-initiatinguser", executionContext.InitiatingUserId.ToString() },
                    { "crm-isintransaction", executionContext.IsInTransaction.ToString() },
                    { "crm-isolationmode", executionContext.IsolationMode.ToString() },
                    { "crm-messagename", executionContext.MessageName },
                    { "crm-mode", executionContext.Mode.ToString() },
                    { "crm-operationid", executionContext.OperationId.ToString()},
                    { "crm-organizationid", executionContext.OrganizationId.ToString() },
                    { "crm-orgname", executionContext.OrganizationName },
                    { "crm-primaryentityid", executionContext.PrimaryEntityId.ToString() },
                    { "crm-primaryentityname", executionContext.PrimaryEntityName },
                    { "crm-requestid", executionContext.RequestId.ToString() },
                    { "crm-stagename", executionContext.StageName },                    
                    { "crm-userid", executionContext.UserId.ToString() }
                 }))
            {
                try
                {
                    var localContextFactory = Container.Resolve<ILocalWorkflowActivityContextFactory>();

                    using (var localContext = localContextFactory.BuildLocalWorkflowActivityContext<E>(executionContext, Container, codeActivityContext, telemetryClient))
                    {
                        try
                        {
                            ExecuteInternal(localContext);
                        }
                        catch (InvalidWorkflowException ex)
                        {
                            if (localContext != null && localContext.TelemetryClient != null && localContext.TelemetryFactory != null)
                            {
                                localContext.TelemetryClient.Track(localContext.TelemetryFactory.BuildMessageTelemetry(ex.Message, SeverityLevel.Error));
                            }
                            throw;
                        }
                        catch (Exception ex)
                        {
                            if (localContext != null && localContext.TelemetryClient != null && localContext.TelemetryFactory != null)
                            {
                                localContext.TelemetryClient.Track(localContext.TelemetryFactory.BuildExceptionTelemetry(ex));
                            }
                            throw;
                        }

                    } //using localContext
                }          
                catch (Exception ex)
                {
                    tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                    throw;
                }

            } //using telemetryClient
        } 
    }
}
