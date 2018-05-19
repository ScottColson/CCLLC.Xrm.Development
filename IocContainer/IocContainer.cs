using System;
using System.Collections.Generic;
using System.Reflection;

namespace CCLCC.Core
{
    /// <summary>
    /// Provides a light weight IOC container implementation for configuring services used by 
    /// CRM plugins.
    /// 
    /// Based on work from Ken Egozi: http://kenegozi.com/blog/2008/01/17/its-my-turn-to-build-an-ioc-container-in-15-minutes-and-33-lines
    /// </summary>
    public class IocContainer : IIocContainer
    {
        private static object lockObject = new object();
        private static IocContainer instance;
        private readonly IDictionary<Type, ImplementationParameters> registeredTypes = new Dictionary<Type, ImplementationParameters>();
        private readonly IDictionary<Type, object> instances = new Dictionary<Type, object>();

        static public IocContainer Instance
        {
            get
            {
                if(instance == null)
                {
                    lock (lockObject)
                    {
                        if(instance == null)
                        {
                            instance = new IocContainer();
                        }
                    }
                }

                return instance;
            }
        }

        public int Count { get { return registeredTypes.Count; } }

        public void Register<TContract, TImplementation>() where TImplementation : TContract
        {
            var implementation = new ImplementationParameters { Type = typeof(TImplementation), SingleInstance = false };
            registeredTypes[typeof(TContract)] = implementation;
        }

        public void RegisterAsSingleInstance<TContract, TImplementation>() where TImplementation : TContract
        {
            var implementation = new ImplementationParameters { Type = typeof(TImplementation), SingleInstance = true };
            registeredTypes[typeof(TContract)] = implementation;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public bool IsRegistered<TContract>()
        {
            return IsRegistered(typeof(TContract));
        }

        private bool IsRegistered(Type contract)
        {
            return registeredTypes.ContainsKey(contract);
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
