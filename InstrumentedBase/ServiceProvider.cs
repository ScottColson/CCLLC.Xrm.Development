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
  
    public class EventTelemetryServiceProvider<T> : IServiceProvider<T> where T : ITelemetryService
    {
        IServiceProvider parentServiceProvider;

        internal EventTelemetryServiceProvider(IServiceProvider parent)
        {
            parentServiceProvider = parent;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ITelemetryProvider<IEventTelemetryService>))
            {
                return new EventTelemetryProvider();
            }

           

            return parentServiceProvider.GetService(serviceType);
        }
    }
}
