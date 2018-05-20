using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Xrm.Sdk;
using CCLCC.Core;
using CCLCC.Telemetry;
using CCLCC.Telemetry.Context;
using CCLCC.Telemetry.Sink;
using CCLCC.Telemetry.Client;
using CCLCC.Telemetry.Serializer;
using CCLCC.Xrm.Sdk.Encryption;
using CCLCC.Xrm.Sdk.Configuration;

namespace CCLCC.Xrm.Sdk
{       
    public abstract class PluginBase<E> : IPlugin<E> where E : Entity
    {
        private static IIocContainer _container;
        private static object _containerLock = new object();
        private static ITelemetrySink _telemetrySink;
        private static object _sinkLock = new object();
        
        private Collection<PluginEvent<E>> events = new Collection<PluginEvent<E>>();
        
        /// <summary>
        /// Provides of list of <see cref="PluginEvent{E}"/> items that define the 
        /// events the plugin can operate against. Add items to the list using the 
        /// <see cref="RegisterEventHandler(string, string, ePluginStage, Action{ILocalContext{E}})"/> method.
        /// </summary>
        public IReadOnlyList<PluginEvent<E>> PluginEventHandlers 
        {
            get
            {                
                return this.events;
            }
        }


        /// <summary>
        /// Provides an <see cref="IIocContainer"/> instance to register all objects used by the
        /// base plugin. This container uses a static implementation therefore all 
        /// plugins that use this base share the same container and therefore
        /// use the same concreate implementations registered in the container.
        /// </summary>
        public virtual IIocContainer Container
        {
            get {
                if (_container == null)
                {
                    lock (_containerLock)
                    {
                        if(_container == null)
                        {
                            _container = new IocContainer();
                            RegisterContainerServices();
                        }
                    }
                   
                }
                return _container;
            }
        }

        /// <summary>
        /// Provides a <see cref="ITelemetrySink"/> to recieve and process various 
        /// <see cref="ITelemetry"/> items generated during the execution of the 
        /// Plugin. This sink uses a static implementation therefore all 
        /// plugins that use this base share the same sink which is more
        /// efficient than operating multiple sinks.
        /// </summary>
        public virtual ITelemetrySink TelemetrySink
        {
            get
            {
                if(_telemetrySink == null)
                {
                    lock (_sinkLock)
                    {
                        if(_telemetrySink == null)
                        {
                            _telemetrySink = Container.Resolve<ITelemetrySink>();
                        }
                    }                    
                }

                return _telemetrySink;
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
        /// Initializes a new instance of the <see cref="PluginBase{E}"/> class 
        /// with configuration information.
        /// </summary>
        public PluginBase(string unsecureConfig, string secureConfig)
        {
            this.UnsecureConfig = unsecureConfig;
            this.SecureConfig = secureConfig;
        }

        /// <summary>
        /// Adds a new event to <see cref="PluginEventHandlers"/> list.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="messageName"></param>
        /// <param name="stage"></param>
        /// <param name="handler"></param>
        public virtual void RegisterEventHandler(string entityName, string messageName, ePluginStage stage, Action<ILocalContext<E>> handler)
        {
            events.Add(new PluginEvent<E>
            {
                EntityName = entityName,
                MessageName = messageName,
                Stage = stage,
                PluginAction = handler
            });
        }

        /// <summary>
        /// Registers all dependencies used by the Plugin. 
        /// </summary>
        public virtual void RegisterContainerServices()
        {
            //Telemetry component registration
            Container.Register<ITelemetryContext, TelemetryContext>();
            Container.Register<ITelemetryClientFactory, TelemetryClientFactory>();
            Container.Register<ITelemetryInitializerChain, TelemetryInitializerChain>();
            Container.Register<ITelemetryChannel, SyncMemoryChannel>();
            Container.Register<ITelemetryBuffer, TelemetryBuffer>();
            Container.Register<ITelemetryTransmitter, AITelemetryTransmitter>();
            Container.Register<ITelemetryProcessChain, TelemetryProcessChain>();
            Container.Register<ITelemetrySink, TelemetrySink>();
            Container.Register<IContextTagKeys, AIContextTagKeys>();  //Context tags known to Application Insights.
            Container.Register<ITelemetrySerializer, AITelemetrySerializer>();
            Container.Register<ITelemetryFactory, TelemetryFactory>();

            //Xrm component registration
            Container.Register<ICacheFactory, CacheFactory>();   
            Container.Register<IConfigurationFactory, ConfigurationFactory>();
            Container.Register<ILocalPluginContextFactory, LocalPluginContextFactory>();
            Container.Register<IRijndaelEncryption, RijndaelEncryption>();
            Container.Register<IExtensionSettingsConfig, DefaultExtensionSettingsConfig>();
        }
        
           

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// Microsoft CRM plugins must be thread-safe and stateless. 
        /// </remarks>
        public virtual void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");
           
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entering {0}.Execute()", this.GetType().ToString()));

            var executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            var telemetryFactory = Container.Resolve<ITelemetryFactory>();
            var telemetryClientFactory = Container.Resolve<ITelemetryClientFactory>();
            
            using (var telemetryClient = telemetryClientFactory.BuildClient(
                this.GetType().ToString(),
                this.TelemetrySink,
                new Dictionary<string, string>{ //Add properties for CRM execution attributes that don't fit cleanly in context.
                    { "crm-depth", executionContext.Depth.ToString() },
                    { "crm-initiatinguser", executionContext.InitiatingUserId.ToString() },
                    { "crm-isintransaction", executionContext.IsInTransaction.ToString() },
                    { "crm-isolationmode", executionContext.IsolationMode.ToString() },                    
                    { "crm-mode", executionContext.Mode.ToString() },                    
                    { "crm-organizationid", executionContext.OrganizationId.ToString() },                    
                    { "crm-requestid", executionContext.RequestId.ToString() },
                    { "crm-stage", executionContext.Stage.ToString() },
                    { "crm-userid", executionContext.UserId.ToString() }
                }))
            {

                #region Setup Telementry Context

                telemetryClient.Context.Operation.Name = executionContext.MessageName;
                telemetryClient.Context.Operation.CorrelationVector = executionContext.CorrelationId.ToString();
                telemetryClient.Context.Operation.Id = executionContext.OperationId.ToString();

                telemetryClient.Context.Session.Id = executionContext.CorrelationId.ToString();

                //not all telemetry context providers support data context. In that case
                //use custom properties.
                var asDataContext = telemetryClient.Context as ISupportDataKeyContext;
                if (asDataContext != null)
                {
                    asDataContext.Data.RecordSource = executionContext.OrganizationName;
                    asDataContext.Data.RecordType = executionContext.PrimaryEntityName;
                    asDataContext.Data.RecordId = executionContext.PrimaryEntityId.ToString();
                }
                else
                {
                    telemetryClient.Context.Properties.Add("crm-recordsource", executionContext.OrganizationName);
                    telemetryClient.Context.Properties.Add("crm-primaryentityid", executionContext.PrimaryEntityId.ToString());
                    telemetryClient.Context.Properties.Add("crm-primaryentityname", executionContext.PrimaryEntityName);
                }

                #endregion
                
                try
                {
                    var matchingHandlers = this.PluginEventHandlers
                        .Where(a => (int)a.Stage == executionContext.Stage
                            && (string.IsNullOrWhiteSpace(a.MessageName) || string.Compare(a.MessageName, executionContext.MessageName, StringComparison.InvariantCultureIgnoreCase) == 0)
                            && (string.IsNullOrWhiteSpace(a.EntityName) || string.Compare(a.EntityName, executionContext.PrimaryEntityName, StringComparison.InvariantCultureIgnoreCase) == 0));

                    if (matchingHandlers.Any())
                    {
                        var localContextFactory = Container.Resolve<ILocalPluginContextFactory>();
                       
                        using (var localContext = localContextFactory.BuildLocalPluginContext<E>(executionContext,  serviceProvider, this.Container, telemetryClient))
                        {                            
                            foreach (var handler in matchingHandlers)
                            {
                                try
                                {
                                    handler.PluginAction.Invoke(localContext);
                                }
                                catch(InvalidPluginExecutionException ex)
                                {
                                    if (telemetryClient != null && telemetryFactory != null)
                                    {
                                        telemetryClient.Track(telemetryFactory.BuildMessageTelemetry(ex.Message, SeverityLevel.Error));
                                    }
                                    throw;
                                }
                                catch(Exception ex)
                                {
                                    
                                    if(telemetryClient != null && telemetryFactory != null)
                                    {
                                        telemetryClient.Track(telemetryFactory.BuildExceptionTelemetry(ex));
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