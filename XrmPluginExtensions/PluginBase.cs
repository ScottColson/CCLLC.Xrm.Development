using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Xrm.Sdk;


namespace CCLCC.XrmPluginExtensions
{ 
    using Context;
    using Diagnostics;
    using Telemetry;

    public abstract class PluginBase<E,T> : IPlugin<E,T> where E : Entity where T : ITelemetryService
    {
        private Collection<PluginEvent<E,T>> events = new Collection<PluginEvent<E, T>>();
        public IReadOnlyList<PluginEvent<E,T>> PluginEventHandlers 
        {
            get
            {                
                return this.events;
            }
        }

        public ITelemetryProvider<T> TelemetryProvider { get; private set; }
        public Dictionary<string,string> TelemetryServiceFactorySettings { get; private set; }

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

        public void RegisterMessageHandler(string entityName, string messageName, ePluginStage stage, Action<ILocalContext<E, T>> handler)
        {
            events.Add(new PluginEvent<E, T>
            {
                EntityName = entityName,
                MessageName = messageName,
                Stage = stage,
                PluginAction = handler
            });
        }


        public virtual IServiceProvider<T> DecorateServiceProvider(IServiceProvider provider)
        {
            return new ServiceProvider<T>(provider);
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

            serviceProvider = DecorateServiceProvider(serviceProvider);
           
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));

            var executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //Setup the plugin telemetry provider. Because this is a plugin instance the telemetry provider must be stateless
            //with respect to an individual plugin Execute event. 
            if(TelemetryProvider == null)
            {
                TelemetryProvider = (ITelemetryProvider<T>)serviceProvider.GetService(typeof(ITelemetryProvider<T>));
                TelemetryProvider.ServiceProviderSettings = () => { return (TelemetryServiceFactorySettings != null) ? TelemetryServiceFactorySettings : new Dictionary<string, string>(); };
            }            
           
            var diagnosticServiceFactory = (IDiagnosticServiceFactory<T>)serviceProvider.GetService(typeof(IDiagnosticServiceFactory<T>));

            using (var diagnosticService = diagnosticServiceFactory.CreateDiagnosticService(this.GetType().ToString(), executionContext, tracingService, this.TelemetryProvider))
            {
                try
                {
                    var matchinHandlers = this.PluginEventHandlers
                        .Where(a => (int)a.Stage == executionContext.Stage
                            && (string.IsNullOrWhiteSpace(a.MessageName) || string.Compare(a.MessageName, executionContext.MessageName, StringComparison.InvariantCultureIgnoreCase) == 0)
                            && (string.IsNullOrWhiteSpace(a.EntityName) || string.Compare(a.EntityName, executionContext.PrimaryEntityName, StringComparison.InvariantCultureIgnoreCase) == 0));

                    if (matchinHandlers.Any())
                    {
                        var localContextFactory = (ILocalContextFactory<T>)serviceProvider.GetService(typeof(ILocalContextFactory<T>));
                        using (var localContext = localContextFactory.CreateLocalPluginContext<E>(executionContext, serviceProvider, diagnosticService))
                        {                            
                            foreach (var handler in matchinHandlers)
                            {
                                diagnosticService.EnterMethod(handler.PluginAction.Method.Name);

                                handler.PluginAction.Invoke(localContext);

                                diagnosticService.ExitMethod();
                            }
                        }
                    }
                }
                catch(InvalidPluginExecutionException ex)
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

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.GetType().ToString()));
        }

        
    }
}