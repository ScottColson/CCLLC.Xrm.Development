using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using CCLLC.Core;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.Sdk
{
    public interface ILocalWorkflowActivityContextFactory     {

        ILocalWorkflowActivityContext BuildLocalWorkflowActivityContext(IWorkflowContext executionContext, IIocContainer container, CodeActivityContext codeActivityContext, IComponentTelemetryClient telemetrClient = null);

    }
}
