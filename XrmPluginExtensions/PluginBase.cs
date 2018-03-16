using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security;

namespace D365.XrmPluginExtensions
{
    using Caching;
    using Configuration;
    using Context;
    using Diagnostics;
    

    public abstract partial class PluginBase : IPlugin
    {
      
        //public class ConfigError
        //{
        //    public const string MISSING_PREIMAGE = "Plugin is missing required preimage.";
        //    public const string MISSING_PREIMAGE_ATTRIBUTE = "Plugin is missing one or more required attributes.";
        //}

        

        

        


        private Collection<PluginEvent> events;
        public Collection<PluginEvent> EventHandlers
        {
            get
            {                 
                if (this.events == null)
                    this.events = new Collection<PluginEvent>();
                return this.events;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref=""/> class.
        /// </summary>
        public PluginBase(string unsecureConfig, string secureConfig)
        {
            this.UnsecureConfig = unsecureConfig;
            this.SecureConfig = secureConfig;
        }
        /// <summary>
        /// Un secure configuration specified during the registration of the plugin step
        /// </summary>
        public string UnsecureConfig { get; private set; }

        /// <summary>
        /// Secure configuration specified during the registration of the plugin step
        /// </summary>
        public string SecureConfig { get; private set; }

        public IServiceProvider OverrideServiceProvider(IServiceProvider provider)
        {
            return new ServiceProvider(provider);
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

            var provider = OverrideServiceProvider(serviceProvider);
           
            var tracingService = (ITracingService)provider.GetService(typeof(ITracingService));
            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));

            var executionContext = (IPluginExecutionContext)provider.GetService(typeof(IPluginExecutionContext));

            var diagnosticServiceFactory = (IDiagnosticServiceFactory)provider.GetService(typeof(IDiagnosticServiceFactory));

            using (var diagnosticService = diagnosticServiceFactory.CreateDiagnosticService(this.GetType(), tracingService, executionContext))
            {
                try
                {
                    var matchinHandlers = this.EventHandlers
                        .Where(a => (int)a.Stage == executionContext.Stage
                            && (string.IsNullOrWhiteSpace(a.MessageName) || string.Compare(a.MessageName, executionContext.MessageName, StringComparison.InvariantCultureIgnoreCase) == 0)
                            && (string.IsNullOrWhiteSpace(a.EntityName) || string.Compare(a.EntityName, executionContext.PrimaryEntityName, StringComparison.InvariantCultureIgnoreCase) == 0));

                        foreach (var handler in matchinHandlers)
                        {
                            diagnosticService.EnterMethod(handler.PluginAction.Method.Name);

                            handler.PluginAction.Invoke(serviceProvider, executionContext, diagnosticService);

                            diagnosticService.ExitMethod();
                        }                   
                }
                catch (Exception ex)
                {
                    diagnosticService.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", ex.ToString()), eSeverityLevel.Error);
                    throw;
                }
                finally
                {
                    
                    diagnosticService.Dispose();
                    tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.GetType().ToString()));
                }
            }
        }
    }
}