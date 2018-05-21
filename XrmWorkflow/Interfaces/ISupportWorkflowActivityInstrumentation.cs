using Microsoft.Xrm.Sdk;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk.Workflow
{
    public interface ISupportWorkflowActivityInstrumentation<E> where E : Entity
    {
        ITelemetrySink TelemetrySink { get; }
        bool ConfigureTelemetrySink(ILocalWorkflowActivityContext<E> localContext);
    }
}


