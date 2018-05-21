using System.Collections.Generic;

namespace CCLLC.Telemetry.Context
{
    public class LocationContext : ILocationContext
    {
        public string Ip { get; set; }

        internal protected LocationContext() { }

        public void CopyTo(ILocationContext target)
        {
            target.Ip = Ip;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.LocationIp, this.Ip, keys.TagSizeLimits);
        }
    }
}
