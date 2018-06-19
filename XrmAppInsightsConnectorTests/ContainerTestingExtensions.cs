namespace XrmAppInsightsConnectorTests
{
    public static class ContainerTestingExtensions
    {
        /// <summary>
        /// Returns true if the container has the interface registered as as singleton.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        public static bool IsSingleton<T>(this CCLLC.Core.IIocContainer container)
        {
            var item1 = container.Resolve<T>();
            var item2 = container.Resolve<T>();
            return item1 != null && item2 != null && item1.Equals(item2);
        }

        /// <summary>
        /// Checks the container to see if the contract is implemented with the 
        /// specified concrete implementation.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="container"></param>
        /// <param name="isSingleton"></param>
        /// <returns></returns>
        public static bool IsRegisteredAs<TContract, TImplementation>(this CCLLC.Core.IIocContainer container, bool isSingleton = false) where TImplementation : class
        {
            var item = container.Resolve<TContract>() as TImplementation;
            if(item == null)
            {
                return false;
            }

            return (container.IsSingleton<TContract>() == isSingleton);
            
        }
    }
}
