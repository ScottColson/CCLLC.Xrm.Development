using System;

namespace CCLLC.Core
{
    public class ContainerContract<TContract> : IContainerContract<TContract>, IContractSettings
    {
        private IocContainer container;
        private bool preexistingRegistration;
        private bool allowOverwrite;
        private bool singleInstance;
        private Type implementation;

        internal ContainerContract(IocContainer container)
        {
            this.container = container;
            this.preexistingRegistration = container.IsRegistered<TContract>();
            this.allowOverwrite = false;
            this.singleInstance = false;
        }

        public IContractSettings Using<TImplementation>() where TImplementation : TContract
        {
            implementation = typeof(TImplementation);
            register();
            return this;
        }

        public IContractSettings WithOverwrite()
        {
            this.allowOverwrite = true;
            register();
            return this;
        }

        public IContractSettings AsSingleInstance()
        {
            this.singleInstance = true;
            register();
            return this;
        }

        private void register()
        {
            if(!preexistingRegistration || allowOverwrite)
            {
                container.RegisterImplementation(typeof(TContract), implementation, singleInstance);
            }
        }
    }
}
