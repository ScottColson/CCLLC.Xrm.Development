using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using CCLLC.Xrm.Sdk;

namespace TelemetrySamples
{
    public class UsingAltPluginBase : AltPluginBase<Entity>
    {
        public UsingAltPluginBase(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
            this.RegisterEventHandler("contact", MessageNames.Create, ePluginStage.PreOperation, MyCreateEventHandler);
        }

        private void MyCreateEventHandler(ILocalPluginContext<Entity> localContext)
        {
            // This line will write to Plugin Trace Log as well as Application Insights
            localContext.Trace("Entered MyCreateEventHandler at {0}", DateTime.Now);
        }
    }
}
