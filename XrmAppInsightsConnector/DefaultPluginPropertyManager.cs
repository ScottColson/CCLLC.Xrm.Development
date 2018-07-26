using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace CCLLC.Xrm.AppInsights
{
    public class DefaultPluginPropertyManager : IXrmTelemetryPropertyManager
    {
        public IDictionary<string, string> CreateXrmPropertiesDictionary(string className, IExecutionContext executionContext)
        {
            var properties =  new Dictionary<string, string>{
                { "source", className },
                { "correlationId", executionContext.CorrelationId.ToString() },
                { "depth", executionContext.Depth.ToString() },
                { "initiatingUserId", executionContext.InitiatingUserId.ToString() },
                { "isInTransaction", executionContext.IsInTransaction.ToString() },
                { "isolationMode", executionContext.IsolationMode.ToString() },
                { "message", executionContext.MessageName },
                { "mode", getModeName(executionContext.Mode) },
                { "operationId", executionContext.OperationId.ToString() },
                { "orgId", executionContext.OrganizationId.ToString() },
                { "orgName", executionContext.OrganizationName },                
                { "requestId", executionContext.RequestId.ToString() },
                { "userId", executionContext.UserId.ToString() },
                { "entityId", executionContext.PrimaryEntityId.ToString() },
                { "entityName", executionContext.PrimaryEntityName }};

            //capture plugin execution context properties as telemetry context properties. 
            var asPluginExecutionContext = executionContext as IPluginExecutionContext;
            if (asPluginExecutionContext != null)
            {
                properties.Add("type", "Plugin");
                properties.Add("stage", getStageName(asPluginExecutionContext.Stage));                
            }

            //capture workflow context properties as telemetry context properties.
            var asWorkflowExecutionContext = executionContext as IWorkflowContext;
            if (asWorkflowExecutionContext != null)
            {
                properties.Add("type", "Workflow");
                properties.Add("stage", asWorkflowExecutionContext.StageName);
                properties.Add("workflowCategory", asWorkflowExecutionContext.WorkflowCategory.ToString());
                properties.Add("workflowMode", asWorkflowExecutionContext.WorkflowMode.ToString());
            }

            return properties;
        }

        private string getModeName(int mode)
        {
            return mode == 0 ? "Synchronus" : "Asynchronus";
        }

        private string getStageName(int stage)
        {
            switch (stage)
            {
                case 10:
                    return "Pre-validation";
                case 20:
                    return "Pre-operation";
                case 40:
                    return "Post-operation";
                default:
                    return "MainOperation";
            }
        }
    }
}
