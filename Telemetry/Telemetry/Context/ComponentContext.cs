using System.Collections.Generic;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Telemetry.Context
{
    using Implementation;

    public class ComponentContext : IComponentContext
    {
        private string version;

        public string Name { get; set; }
        public string Version { get; set; }
        
        public void UpdateTags(IDictionary<string, string> tags)
        {
            tags.UpdateTagValue(ContextTagKeys.Keys.ApplicationVersion, this.Version);
        }

        public void CopyTo(IComponentContext target) 
        {
            Tags.CopyTagValue(this.Version, target.Version);
        }

      
    }
}
