using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Xrm.Sdk;
using CCLLC.Core;
using CCLLC.Telemetry;
using CCLLC.Telemetry.Context;
using CCLLC.Telemetry.Sink;
using CCLLC.Telemetry.Client;
using CCLLC.Telemetry.Serializer;
using CCLLC.Xrm.Sdk.Encryption;
using CCLLC.Xrm.Sdk.Configuration;

namespace CCLLC.Xrm.Sdk
{       
    public abstract class PluginBase<E> : IPlugin<E> where E : Entity
    {
        private static IIocContainer _container;
        private static object _containerLock = new object();
        
        
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
            
           
                try
                {
                    var matchingHandlers = this.PluginEventHandlers
                        .Where(a => (int)a.Stage == executionContext.Stage
                            && (string.IsNullOrWhiteSpace(a.MessageName) || string.Compare(a.MessageName, executionContext.MessageName, StringComparison.InvariantCultureIgnoreCase) == 0)
                            && (string.IsNullOrWhiteSpace(a.EntityName) || string.Compare(a.EntityName, executionContext.PrimaryEntityName, StringComparison.InvariantCultureIgnoreCase) == 0));

                    if (matchingHandlers.Any())
                    {
                        var localContextFactory = Container.Resolve<ILocalPluginContextFactory>();
                       
                        using (var localContext = localContextFactory.BuildLocalPluginContext<E>(executionContext,  serviceProvider, this.Container, null))
                        {
                            foreach (var handler in matchingHandlers)
                            {                               
                                    handler.PluginAction.Invoke(localContext);                                
                            }
                        } //using localContext
                    }
                }
                catch (Exception ex)
                {
                    tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                    throw;
                }              
                
           

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.GetType().ToString()));
        }

        
      
    }
}