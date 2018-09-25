using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Xrm.Sdk;
using CCLLC.Core;
using CCLLC.Xrm.Sdk.Encryption;
using CCLLC.Xrm.Sdk.Configuration;

namespace CCLLC.Xrm.Sdk
{
    /// <summary>
    /// Base plugin class for plugins using <see cref="IEnhancedPlugin"/> functionality. This class does not provide
    /// telemetry logging outside of Dynamics 365. For external telemetry use <see cref="InstrumentedPluginBase"/>."/>
    /// </summary>
    public abstract class PluginBase : IPlugin, IEnhancedPlugin
    {
        private Collection<PluginEvent> _events = new Collection<PluginEvent>();
        private IIocContainer _container = null;

        /// <summary>
        /// Provides of list of <see cref="PluginEvent"/> items that define the 
        /// events the plugin can operate against. Add items to the list using the 
        /// <see cref="RegisterEventHandler(string, string, ePluginStage, Action{ILocalPluginContext}, string)"/> method.
        /// </summary>
        public IReadOnlyList<PluginEvent> PluginEventHandlers
        {
            get
            {
                return this._events;
            }
        }


        /// <summary>
        /// Provides an <see cref="IIocContainer"/> instance to register all objects used by the
        /// base plugin. 
        /// </summary>
        public virtual IIocContainer Container
        {
            get
            {                
                if (_container == null) { _container = new IocContainer(); }
                return _container;
            }
        }


        /// <summary>
        /// Unsecure configuration specified during the registration of the plugin step
        /// </summary>
        public string UnsecureConfig { get; }

        /// <summary>
        /// Secure configuration specified during the registration of the plugin step
        /// </summary>
        public string SecureConfig { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="PluginBase"/> class 
        /// with configuration information.
        /// </summary>
        public PluginBase(string unsecureConfig, string secureConfig)
        {
            this.UnsecureConfig = unsecureConfig;
            this.SecureConfig = secureConfig;
            RegisterContainerServices();
        }

        /// <summary>
        /// Adds a new event to <see cref="PluginEventHandlers"/> list.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="messageName"></param>
        /// <param name="stage"></param>
        /// <param name="handler"></param>
        /// <param name="id"></param>
        public virtual void RegisterEventHandler(string entityName, string messageName, ePluginStage stage, Action<ILocalPluginContext> handler, string id="")
        {
            this._events.Add(new PluginEvent
            {
                EntityName = entityName,
                MessageName = messageName,
                Stage = stage,
                PluginAction = handler,
                Id = id
            });
        }

        /// <summary>
        /// Registers all dependencies used by the Plugin. 
        /// </summary>
        public virtual void RegisterContainerServices()
        {
            //Xrm component registration    
            Container.Implement<ICacheFactory>().Using<CacheFactory>();
            Container.Implement<IConfigurationFactory>().Using<ConfigurationFactory>();
            Container.Implement<ILocalPluginContextFactory>().Using<LocalPluginContextFactory>();
            Container.Implement<IRijndaelEncryption>().Using<RijndaelEncryption>();
            Container.Implement<IExtensionSettingsConfig>().Using<DefaultExtensionSettingsConfig>();
            Container.Implement<IPluginWebRequestFactory>().Using<Utilities.PluginHttpWebRequestFactory>();
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
            
            try
            {
                var matchingHandlers = this.PluginEventHandlers
                    .Where(a => (int)a.Stage == executionContext.Stage
                        && (string.IsNullOrWhiteSpace(a.MessageName) || string.Compare(a.MessageName, executionContext.MessageName, StringComparison.InvariantCultureIgnoreCase) == 0)
                        && (string.IsNullOrWhiteSpace(a.EntityName) || string.Compare(a.EntityName, executionContext.PrimaryEntityName, StringComparison.InvariantCultureIgnoreCase) == 0));

                if (matchingHandlers.Any())
                {
                    var localContextFactory = Container.Resolve<ILocalPluginContextFactory>();

                    using (var localContext = localContextFactory.BuildLocalPluginContext(executionContext, serviceProvider, this.Container, null))
                    {
                        foreach (var handler in matchingHandlers)
                        {
                            handler.PluginAction.Invoke(localContext);
                        }
                    }
                }
            }
            catch(InvalidPluginExecutionException ex)
            {
                tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                throw new InvalidPluginExecutionException(string.Format("Unhandled Plugin Exception {0}", ex.Message), ex);
            }

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.GetType().ToString()));
        }
    }
}