using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface ICloudContext 
    {
        string RoleName { get; set; }
        string RoleInstance { get; set; }
        
        void CopyTo(ICloudContext target);
        void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys);

    }
}
