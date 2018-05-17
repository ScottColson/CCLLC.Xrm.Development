using System.Collections.Generic;

namespace CCLCC.Telemetry.Context
{
    public class InternalContext : IInternalContext
    {
        public string SdkVersion { get; set; }
        public string AgentVersion { get; set; }
        public string NodeName { get; set; }

        internal protected InternalContext() { }

        public void CopyTo(IInternalContext target)
        {
            target.SdkVersion = SdkVersion;
            target.AgentVersion = AgentVersion;
            target.NodeName = NodeName;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.InternalSdkVersion, this.SdkVersion, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.InternalAgentVersion, this.AgentVersion, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.InternalNodeName, this.NodeName, keys.TagSizeLimits);
        }
    }
}
