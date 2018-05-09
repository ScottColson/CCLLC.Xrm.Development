using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;
using CCLCC.Telemetry.DataContract;



namespace CCLCC.Xrm
{
    using Caching;
    using Configuration;
    using Context;
    using Encryption;
   
    public abstract class PluginBase<E> : IPlugin<E> where E : Entity
    {
        private Collection<PluginEvent<E>> events = new Collection<PluginEvent<E>>();
       
        public IReadOnlyList<PluginEvent<E>> PluginEventHandlers 
        {
            get
            {                
                return this.events;
            }
        }


        private IIocContainer container;
        public IIocContainer Container
        {
            get {
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
                if(telemetrySink == null)
                {
                    telemetrySink = Container.Resolve<ITelemetrySink>();
                }
                return telemetrySink;
            }
        }

        /// <summary>
        /// Unsecure configuration specified during the registration of the plugin step
        /// </summary>
        public string UnsecureConfig { get; private set; }

        /// <summary>
        /// Secure configuration specified during the registration of the plugin step
        /// </summary>
        public string SecureConfig { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref=""/> class.
        /// </summary>
        public PluginBase(string unsecureConfig, string secureConfig)
        {
            this.UnsecureConfig = unsecureConfig;
            this.SecureConfig = secureConfig;
        }

        public PluginBase() { }

        public void RegisterMessageHandler(string entityName, string messageName, ePluginStage stage, Action<ILocalContext<E>> handler)
        {
            events.Add(new PluginEvent<E>
            {
                EntityName = entityName,
                MessageName = messageName,
                Stage = stage,
                PluginAction = handler
            });
        }

        public virtual void RegisterContainerServices(IIocContainer container)
        {
            container.Register<ITelemetrySink, CCLCC.Telemetry.Sink.TelemetrySink>();
            container.Register<ITelemetryFactory, CCLCC.Telemetry.TelemetryFactory>();
            container.Register<IApplicationTelemetryClientFactory, IApplicationTelemetryClientFactory>();
            container.Register<ICacheFactory, CacheFactory>();   
            container.Register<IConfigurationFactory, ConfigurationFactory>();
            container.Register<ILocalPluginContextFactory, LocalPluginContextFactory>();
            container.Register<IRijndaelEncryption, RijndaelEncryption>();
        }
        

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// Microsoft CRM plugins must be thread-safe and stateless. 
        /// </remarks>
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");
           
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entering {0}.Execute()", this.GetType().ToString()));

            var executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            var telemetryFactory = Container.Resolve<ITelemetryFactory>();
            var telemetryClientFactory = Container.Resolve<IApplicationTelemetryClientFactory>();

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
                    { "crm-stage", executionContext.Stage.ToString() },
                    { "crm-userid", executionContext.UserId.ToString() }
                }))
            {                
                try
                {
                    var matchingHandlers = this.PluginEventHandlers
                        .Where(a => (int)a.Stage == executionContext.Stage
                            && (string.IsNullOrWhiteSpace(a.MessageName) || string.Compare(a.MessageName, executionContext.MessageName, StringComparison.InvariantCultureIgnoreCase) == 0)
                            && (string.IsNullOrWhiteSpace(a.EntityName) || string.Compare(a.EntityName, executionContext.PrimaryEntityName, StringComparison.InvariantCultureIgnoreCase) == 0));

                    if (matchingHandlers.Any())
                    {
                        var localContextFactory = Container.Resolve<ILocalPluginContextFactory>();
                       
                        using (var localContext = localContextFactory.CreateLocalPluginContext<E>(executionContext,  serviceProvider, this.Container, telemetryClient))
                        {                            
                            foreach (var handler in matchingHandlers)
                            {
                                try
                                {
                                    handler.PluginAction.Invoke(localContext);
                                }
                                catch(InvalidPluginExecutionException ex)
                                {
                                    if (localContext != null && localContext.TelemetryClient != null && localContext.TelemetryFactory != null)
                                    {
                                        localContext.TelemetryClient.Track(localContext.TelemetryFactory.BuildMessageTelemetry(ex.Message, SeverityLevel.Error));
                                    }
                                    throw;
                                }
                                catch(Exception ex)
                                {
                                    
                                    if(localContext != null && localContext.TelemetryClient != null && localContext.TelemetryFactory != null)
                                    {
                                        localContext.TelemetryClient.Track(localContext.TelemetryFactory.BuildExceptionTelemetry(ex));
                                    }
                                    throw;
                                }
                            }
                        } //using localContext
                    }
                }
                catch (Exception ex)
                {
                    tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                    throw;
                }
                
            } //using telemetryClient.

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.GetType().ToString()));
        }

        
    }
}