using System.Collections.Generic;

namespace CCLCC.Telemetry.Context
{
    public class CloudContext : ICloudContext
    {       
        public string RoleName { get; set; }       
        public string RoleInstance { get; set; }

        internal protected CloudContext()
        {
        }        

        public void CopyTo(ICloudContext target)
        {
            RoleName = target.RoleName;
            RoleInstance = target.RoleInstance;            
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.CloudRole, this.RoleName, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.CloudRoleInstance, this.RoleInstance, keys.TagSizeLimits);
        }
    }
}
