using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace CCLLC.Xrm.Sdk.Telemetry
{
    public class ExecutionContextPropertyManager : IXrmTelemetryPropertyManager
    {
        public virtual IDictionary<string, string> CreateXrmPropertiesDictionary(string className, IExecutionContext executionContext)
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

            return properties;
        }

        protected string getModeName(int mode)
        {
            return mode == 0 ? "Synchronus" : "Asynchronus";
        }

        protected string getStageName(int stage)
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
