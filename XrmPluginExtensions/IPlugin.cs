using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.XrmPluginExtensions
{
    public interface IPlugin : Microsoft.Xrm.Sdk.IPlugin
    {
        Collection<PluginEvent> EventHandlers { get; }
        string UnsecureConfig { get; }
        string SecureConfig { get; }

        IServiceProvider OverrideServiceProvider(IServiceProvider provider);
    }
}
