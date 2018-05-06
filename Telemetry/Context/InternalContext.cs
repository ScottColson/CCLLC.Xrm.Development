using System.Collections.Generic;
using CCLCC.Telemetry.Implementation;
using CCLCC.Telemetry.Interfaces;

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
            SdkVersion = target.SdkVersion;
            AgentVersion = target.AgentVersion;
            NodeName = target.NodeName;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.InternalSdkVersion, this.SdkVersion, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.InternalAgentVersion, this.AgentVersion, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.InternalNodeName, this.NodeName, keys.TagSizeLimits);
        }
    }
}
