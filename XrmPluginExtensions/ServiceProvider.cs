using System;
using Microsoft.Xrm.Sdk;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions
{

    using Caching;
    using Configuration;
    using Context;
    using Diagnostics;
    using Encryption;
    using Telemetry;
  
    public class ServiceProvider<T> : IServiceProvider<T> where T : ITelemetryService
    {
        IServiceProvider parentServiceProvider;

        internal ServiceProvider(IServiceProvider parent)
        {
            parentServiceProvider = parent;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ITelemetryProvider<T>))
            {
                return new TracingTelemetryProvider();
            }

            if (serviceType == typeof(ICacheFactory))
            {
                return new CacheFactory();
            }

            if (serviceType == typeof(IConfigurationFactory))
            {
                return new ConfigurationFactory();
            }

            if (serviceType == typeof(IDiagnosticServiceFactory<T>))
            {
                return new DiagnosticServiceFactory<T>();
            }

            if (serviceType == typeof(ILocalContextFactory<T>))
            {
                return new LocalContextFactory<T>();
            }

            if (serviceType == typeof(IRijndaelEncryption))
            {
                return new RijndaelEncryption();
            }

            return parentServiceProvider.GetService(serviceType);
        }
    }
}
