using System;
using Microsoft.Xrm.Sdk;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.XrmPluginExtensions
{

    using Caching;
    using Configuration;
    using Diagnostics;
    using Encryption;
  
    public class ServiceProvider : IServiceProvider
    {
        IServiceProvider parentServiceProvider;

        internal ServiceProvider(IServiceProvider parent)
        {
            parentServiceProvider = parent;
        }

        public object GetService(Type serviceType)
        {       
           if(serviceType== typeof(ICacheFactory))
            {
                return new CacheFactory();
            }

            if (serviceType == typeof(IConfigurationFactory))
            {
                return new ConfigurationFactory();
            }

            if (serviceType == typeof(IDiagnosticServiceFactory))
            {
                return new DiagnosticServiceFactory();
            }
            if (serviceType == typeof(IRijndaelEncryption))
            {
                return new RijndaelEncryption();
            }

            return parentServiceProvider.GetService(serviceType);
        }
    }
}
