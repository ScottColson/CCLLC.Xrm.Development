using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using CCLLC.Xrm.Sdk.Workflow;
using CCLLC.Xrm.Sdk;

namespace CCLLCExtensionSettings
{
    public class GetExtensionSettingAsStringWFA : WorkflowActivityBase
    {    
        [RequiredArgument]
        [Input("Setting Name")]
        public InArgument<string> SettingName { get; set; }

        [RequiredArgument]
        [Input("Default Value")]
        public InArgument<string> DefaultValue { get; set; }

        [Output("Setting Value")]
        public OutArgument<string> SettingValue { get; set; }   

        public override void ExecuteInternal(ILocalWorkflowActivityContext localContext)
        {
            //get the passed in value name and default return
            var key = this.SettingName.Get(localContext.CodeActivityContext);
            var defaultValue = this.DefaultValue.Get(localContext.CodeActivityContext);

            //pull from ExtensionSettings and return default if value is not in the settings.
            var value = localContext.ExtensionSettings.Get<string>(key, defaultValue);

            //return the value
            this.SettingValue.Set(localContext.CodeActivityContext, value);
        }
    }
}
