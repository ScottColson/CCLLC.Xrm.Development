using Microsoft.Xrm.Sdk;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk
{
    public interface ISupportPluginInstrumentation<E> where E : Entity
    {
        ITelemetrySink TelemetrySink { get; }
        bool ConfigureTelemetrySink(ILocalPluginContext<E> localContext);
    }
}
