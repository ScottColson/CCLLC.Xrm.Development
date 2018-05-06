using System.Collections.Generic;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Context
{
    public class ComponentContext : IComponentContext
    {      
        public string Name { get; set; }
        public string Version { get; set; }

        internal protected ComponentContext() { }

        public void CopyTo(IComponentContext target)
        {
            Name = target.Name;
            Version = target.Version;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.ComponentName, this.Name, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.ComponentVersion, this.Version, keys.TagSizeLimits);
            
        }
        
    }
}
