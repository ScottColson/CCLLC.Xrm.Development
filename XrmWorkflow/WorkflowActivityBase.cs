using System;
using System.Activities;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using CCLCC.Core;


namespace CCLCC.Xrm.Sdk.Workflow
{
    
    using Configuration;
    using Encryption;
    using System.Collections.Generic;

    public abstract class WorkflowActivityBase<E> : CodeActivity, IWorkflowActivity<E> where E : Entity
    {
        private static IIocContainer _container;
        private static object _containerLock = new object();


        /// <summary>
        /// Provides an <see cref="IIocContainer"/> instance to register all objects used by the
        /// base workflow activity. This container uses a static implementation therefore all 
        /// workflow activities that use this base share the same container and therefore
        /// use the same concreate implementations registered in the container.
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
        /// Registers all dependencies used by the WorkflowActivity. 
        /// </summary>
        public virtual void RegisterContainerServices()
        {
            //Xrm component registration
            Container.Register<IExtensionSettingsConfig, DefaultExtensionSettingsConfig>();
            Container.Register<ICacheFactory, CacheFactory>();
            Container.Register<IConfigurationFactory, ConfigurationFactory>();
            Container.Register<ILocalWorkflowActivityContextFactory, LocalWorkflowActivityContextFactory>();
            Container.Register<IRijndaelEncryption, RijndaelEncryption>();
        }



        public abstract void ExecuteInternal(ILocalWorkflowActivityContext<E> localContext);

        protected override void Execute(CodeActivityContext codeActivityContext)
        {
            if (codeActivityContext == null) { throw new ArgumentNullException("codeActivityContext"); }

            var tracingService = codeActivityContext.GetExtension<ITracingService>();

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));


            var executionContext = codeActivityContext.GetExtension<IWorkflowContext>();

            try
            {
                var localContextFactory = Container.Resolve<ILocalWorkflowActivityContextFactory>();

                using (var localContext = localContextFactory.BuildLocalWorkflowActivityContext<E>(executionContext, Container, codeActivityContext))
                {

                    ExecuteInternal(localContext);


                } //using localContext
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
    }
}
