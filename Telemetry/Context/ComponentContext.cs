using System.Collections.Generic;

namespace CCLLC.Telemetry.Context
{
    public class ComponentContext : IComponentContext
    {      
        public string Name { get; set; }
        public string Version { get; set; }

        internal protected ComponentContext() { }

        public void CopyTo(IComponentContext target)
        {
            target.Name = Name;
            target.Version = Version;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.ComponentName, this.Name, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.ComponentVersion, this.Version, keys.TagSizeLimits);
            
        }
        
    }
}
