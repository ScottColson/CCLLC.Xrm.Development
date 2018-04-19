using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase
{
    using Caching;
    using Configuration;
    using Container;
    using Context;
    using Encryption;
    using Telemetry;
    using Telemetry.Providers;

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

        private IContainer container;
        public IContainer Container
        {
            get {
                if (container == null)
                {
                    container = new Container.Container();
                    RegisterContainerServices(container);
                }
                return container;
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

        public virtual void RegisterContainerServices(IContainer container)
        {
            container.RegisterAsSingleInstance<ITelemetryProvider, DefaultTelemetryProvider>();
            container.RegisterAsSingleInstance<ICacheFactory, CacheFactory>();   
            container.RegisterAsSingleInstance<IConfigurationFactory, ConfigurationFactory>();
            container.RegisterAsSingleInstance<ILocalPluginContextFactory, LocalPluginContextFactory>();
            container.RegisterAsSingleInstance<IRijndaelEncryption, RijndaelEncryption>();
        }

        public virtual ConfigureTelemtryProvider GetConfigureTelemetryProviderCallback(ILocalContext<E> localContext)
        {
            return (p) => p.Configure(null);
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

            var telemetryProvider = Container.Resolve<ITelemetryProvider>();

            using (var telemetryService = telemetryProvider.CreateTelemetryService(this.GetType().ToString(), tracingService, telemetryProvider, executionContext))
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
                       
                        using (var localContext = localContextFactory.CreateLocalPluginContext<E>(executionContext, Container, serviceProvider, telemetryService))
                        {
                            localContext.SetConfigureTelemetryProviderCallback(GetConfigureTelemetryProviderCallback(localContext));

                            foreach (var handler in matchingHandlers)
                            {
                                handler.PluginAction.Invoke(localContext);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    telemetryService.TraceException(ex);
                    throw;
                }
                
            }

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.GetType().ToString()));
        }

        
    }
}