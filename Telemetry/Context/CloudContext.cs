using System.Collections.Generic;

namespace CCLLC.Telemetry.Context
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
            target.RoleName = RoleName;
            target.RoleInstance = RoleInstance;            
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.CloudRole, this.RoleName, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.CloudRoleInstance, this.RoleInstance, keys.TagSizeLimits);
        }
    }
}
