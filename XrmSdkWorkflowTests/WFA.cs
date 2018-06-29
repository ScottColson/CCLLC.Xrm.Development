
using CCLLC.Xrm.Sdk;
using CCLLC.Xrm.Sdk.Workflow;


namespace XrmSdkWorkflowTests
{
    public class WFA : WorkflowActivityBase
    {
        public WFA() : base() { }

        public override void ExecuteInternal(ILocalWorkflowActivityContext localContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
