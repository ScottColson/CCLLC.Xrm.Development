using System;
using System.Collections.Generic;
using System.Reflection;

namespace CCLLC.Core
{
    /// <summary>
    /// Provides a light weight IOC container implementation for configuring services used by 
    /// CRM plugins. Based on work from Ken Egozi: http://kenegozi.com/blog/2008/01/17/its-my-turn-to-build-an-ioc-container-in-15-minutes-and-33-lines
    /// </summary>
    public class IocContainer : IIocContainer
    {
        private static object lockObject = new object();
        private readonly IDictionary<Type, ImplementationParameters> registeredTypes = new Dictionary<Type, ImplementationParameters>();
        private readonly IDictionary<Type, object> instances = new Dictionary<Type, object>();        
        
        /// <summary>
        /// The number of items registered in the container.
        /// </summary>
        public int Count { get { return registeredTypes.Count; } }

        /// <summary>
        /// Register 
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        public IContainerContract<TContract> Implement<TContract>()
        {
            IContainerContract<TContract> contract = new ContainerContract<TContract>(this);
            return contract;
        }

        /// <summary>
        /// Register an implementation for a given interface contract.
        /// </summary>
        /// <typeparam name="TContract">The type of the interface contract.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        [Obsolete("Use Implement<TContract>().Using<TImplementation>() fluent registration builder which prevents accidental overwrite on an existing implementation. Use .WithOverwrite() to overwrite an existing implementation.")]
        public void Register<TContract, TImplementation>() where TImplementation : TContract
        {
            RegisterImplementation(typeof(TContract), typeof(TImplementation), false);            
        }

        /// <summary>
        /// Register an single instance implementation for given interface contract. When
        /// an implementation is registers as a single instance, the container creates a
        /// singleton for that implementation so the same object is returned for each
        /// call to <see cref="Resolve{T}"/>.  
        /// </summary>
        /// <typeparam name="TContract">The type of the interface contract.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        [Obsolete("Use Implement<TContract>().Using<TImplementation>().AsSingleInstance() fluent registration builder which prevents accidental overwrite on an existing implementation. Use .WithOverwrite() to overwrite an existing implementation.")]
        public void RegisterAsSingleInstance<TContract, TImplementation>() where TImplementation : TContract
        {
            RegisterImplementation(typeof(TContract), typeof(TImplementation), true);
        }

        /// <summary>
        /// Return an implementation for the desired contract interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface contract.</typeparam>
        /// <returns>The registered implementation as the requested interface contract.</returns>
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        /// Check to see if a given contract is already registered in the container.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns>Returns true if the interface contract is already registered.</returns>
        public bool IsRegistered<TContract>()
        {
            return IsRegistered(typeof(TContract));
        }

        private bool IsRegistered(Type contract)
        {
            return registeredTypes.ContainsKey(contract);
        }

        internal void RegisterImplementation(Type contract, Type implementation, bool singleInstance)
        {
            var implementationParam = new ImplementationParameters { Type = implementation, SingleInstance = singleInstance };
            registeredTypes[contract] = implementationParam;

            //if this implementation has been rendered for a single instance then clear that instance now.
            if (instances.ContainsKey(implementation))
            {
                instances.Remove(implementation);
            }
        }

        private object Resolve(Type contract)
        {
            if (!IsRegistered(contract))
            {
                throw new Exception(string.Format("Type {0} is not registered in the container.", contract.ToString()));
            }

            var implementation = registeredTypes[contract];

            if(implementation.SingleInstance && instances.ContainsKey(implementation.Type))
            {
                return instances[implementation.Type];
            }

            ConstructorInfo constructor = implementation.Type.GetConstructors()[0];
            ParameterInfo[] constructorParameters = constructor.GetParameters();
            if (constructorParameters.Length == 0)
            {
                var i = Activator.CreateInstance(implementation.Type);
                if (implementation.SingleInstance)
                {
                    instances[implementation.Type] = i;
                }
                return i;
            }
            else
            {
                List<object> parameters = new List<object>(constructorParameters.Length);
                foreach (ParameterInfo parameterInfo in constructorParameters)
                {
                    parameters.Add(Resolve(parameterInfo.ParameterType));
                }

                var i = constructor.Invoke(parameters.ToArray());
                if (implementation.SingleInstance)
                {
                    instances[implementation.Type] = i;
                }
                return i;
            }
        }
    }
}
