using System;
using CCLLC.Xrm.Sdk;

namespace XrmSdkWorkflowTests
{
    public class InstrumentedWFA : CCLLC.Xrm.Sdk.Workflow.InstrumentedWorkflowActivityBase
    {
        public InstrumentedWFA() : base() { }

        public override void ExecuteInternal(ILocalWorkflowActivityContext localContext)
        {
            throw new NotImplementedException();
        }
    }
}
