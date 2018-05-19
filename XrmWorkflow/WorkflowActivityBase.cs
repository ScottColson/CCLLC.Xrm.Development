using System;
using System.Activities;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLCC.Core;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Sdk.Workflow
{
    using Caching;
    using CCLCC.Telemetry.Client;
    using CCLCC.Telemetry.Context;
    using CCLCC.Telemetry.Serializer;
    using CCLCC.Telemetry.Sink;
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
            //Telemetry component registration
            container.Register<ITelemetryContext, TelemetryContext>();
            container.Register<ITelemetryClientFactory, TelemetryClientFactory>();
            container.Register<ITelemetryInitializerChain, TelemetryInitializerChain>();
            container.Register<ITelemetryChannel, SyncMemoryChannel>();
            container.Register<ITelemetryBuffer, TelemetryBuffer>();
            container.Register<ITelemetryTransmitter, TelemetryTransmitter>();
            container.Register<ITelemetryProcessChain, TelemetryProcessChain>();
            container.Register<ITelemetrySink, TelemetrySink>();
            container.Register<IContextTagKeys, AIContextTagKeys>();  //Context tags known to Application Insights.
            container.Register<ITelemetrySerializer, AITelemetrySerializer>();
            container.Register<ITelemetryFactory, TelemetryFactory>();

            //Xrm component registration
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
            var telemetryClientFactory = Container.Resolve<ITelemetryClientFactory>();

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
                    telemetryClient.Context.Operation.Name = executionContext.MessageName;
                    telemetryClient.Context.Operation.CorrelationVector = executionContext.CorrelationId.ToString();
                    telemetryClient.Context.Operation.Id = executionContext.OperationId.ToString();

                    var asDataContext = telemetryClient.Context as ISupportDataKeyContext;
                    if (asDataContext != null)
                    {
                        asDataContext.Data.RecordSource = executionContext.OrganizationName;
                        asDataContext.Data.RecordType = executionContext.PrimaryEntityName;
                        asDataContext.Data.RecordId = executionContext.PrimaryEntityId.ToString();
                    }
                }

                finally { }
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
