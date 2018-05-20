using Microsoft.Xrm.Sdk;
using CCLCC.Telemetry;

namespace CCLCC.Xrm.Sdk.Workflow
{
    public interface ISupportWorkflowActivityInstrumentation<E> where E : Entity
    {
        ITelemetrySink TelemetrySink { get; }
        bool ConfigureTelemetrySink(ILocalWorkflowActivityContext<E> localContext);
    }
}


