using Microsoft.Xrm.Sdk;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Sdk
{
    public interface ISupportPluginInstrumentation<E> where E : Entity
    {
        ITelemetrySink TelemetrySink { get; }
        bool ConfigureTelemetrySink(ILocalPluginContext<E> localContext);
    }
}
