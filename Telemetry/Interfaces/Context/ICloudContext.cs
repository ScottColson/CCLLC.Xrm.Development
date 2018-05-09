using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
{
    public interface ICloudContext 
    {
        string RoleName { get; set; }
        string RoleInstance { get; set; }
        
        void CopyTo(ICloudContext target);
        void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys);

    }
}
