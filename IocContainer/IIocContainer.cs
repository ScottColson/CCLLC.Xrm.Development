namespace CCLLC.Core
{
    /// <summary>
    /// Defines a standard interface for registering interface implementations and resolving
    /// them at runtime.
    /// </summary>
    public interface IIocContainer
    {
        /// <summary>
        /// Register an implementation for a given interface contract.
        /// </summary>
        /// <typeparam name="TContract">The type of the interface contract.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void Register<TContract, TImplementation>() where TImplementation : TContract;

        /// <summary>
        /// Register an single instance implementation for given interface contract. When
        /// an implementation is registers as a single instance, the container creates a
        /// singleton for that implementation so the same object is returned for each
        /// call to <see cref="Resolve{T}"/>.  
        /// </summary>
        /// <typeparam name="TContract">The type of the interface contract.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void RegisterAsSingleInstance<TContract, TImplementation>() where TImplementation : TContract;

        /// <summary>
        /// Return an implementation for the desired contract interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface contract.</typeparam>
        /// <returns>The registered implementation as the requested interface contract.</returns>
        T Resolve<T>();

        /// <summary>
        /// Check to see if a given contract is already registered in the container.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns>Returns true if the interface contract is already registered.</returns>
        bool IsRegistered<TContract>();

        /// <summary>
        /// The number of items registered in the container.
        /// </summary>
        int Count { get; }
    }
}
