using System;
using System.Activities;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLLC.Core;
using CCLLC.Xrm.Sdk.Configuration;
using CCLLC.Xrm.Sdk.Encryption;

namespace CCLLC.Xrm.Sdk.Workflow
{    
    public abstract partial class WorkflowActivityBase : CodeActivity, IWorkflowActivity
    {
        private static IIocContainer _container;
        private static object _containerLock = new object();

        /// <summary>
        /// Provides an <see cref="IIocContainer"/> instance to register all objects used by
        /// <see cref="WorkflowActivityBase"/>. This container uses a thread-safe singleton 
        /// implementation. Therefore all workflow activities that use this base share the 
        /// same <see cref="IIocContainer"/> within a given process space and will therefore 
        /// use the same concreate implementations for registered dependencies. 
        /// </summary>
        public virtual IIocContainer Container
        {
            get
            {
                if (_container == null)
                {
                    lock (_containerLock)
                    {
                        if (_container == null)
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
        /// Registers implementations for all dependencies used by <see cref="WorkflowActivityBase"/>. 
        /// Override this method to add additional dependencies or to change the implementation associated
        /// with the items registered here.
        /// </summary>
        public virtual void RegisterContainerServices()
        {            
            Container.Register<IExtensionSettingsConfig, DefaultExtensionSettingsConfig>();
            Container.Register<ICacheFactory, CacheFactory>();
            Container.Register<IConfigurationFactory, ConfigurationFactory>();
            Container.Register<ILocalWorkflowActivityContextFactory, LocalWorkflowActivityContextFactory>();
            Container.Register<IRijndaelEncryption, RijndaelEncryption>();
            Container.Register<IPluginWebRequestFactory, CCLLC.Xrm.Sdk.Utilities.PluginHttpWebRequestFactory>();
        }

        /// <summary>
        /// The handler called by CRM when a WFA is executed. Spins up a <see cref="ILocalWorkflowActivityContext"/>
        /// instance and passes it to the <see cref="ExecuteInternal(ILocalWorkflowActivityContext)"/> method
        /// which is implemented in the inheriting class.
        /// </summary>
        /// <param name="codeActivityContext">Context information passed in by CRM.</param>
        protected override void Execute(CodeActivityContext codeActivityContext)
        {
            if (codeActivityContext == null) { throw new ArgumentNullException("codeActivityContext"); }

            var tracingService = codeActivityContext.GetExtension<ITracingService>();

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));
            
            var executionContext = codeActivityContext.GetExtension<IWorkflowContext>();

            try
            {
                var localContextFactory = Container.Resolve<ILocalWorkflowActivityContextFactory>();

                using (var localContext = localContextFactory.BuildLocalWorkflowActivityContext(executionContext, Container, codeActivityContext))
                {
                    ExecuteInternal(localContext);
                } 
            }
            catch (Exception ex)
            {
                if (tracingService != null)
                {
                    tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                }
                throw;
            }
        }
        
        /// <summary>
        /// Handler that must be implemented in inheriting class to do the actual work of the WFA.
        /// </summary>
        /// <param name="localContext">Instance of <see cref="ILocalWorkflowActivityContext"/> passed 
        /// in from <see cref="Execute(CodeActivityContext)"/>.</param>
        public abstract void ExecuteInternal(ILocalWorkflowActivityContext localContext);

    }
}
