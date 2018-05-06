using System.Collections.Generic;
using CCLCC.Telemetry.Implementation;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Context
{
    public class LocationContext : ILocationContext
    {
        public string Ip { get; set; }

        internal protected LocationContext() { }

        public void CopyTo(ILocationContext target)
        {
            Ip = target.Ip;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.LocationIp, this.Ip, keys.TagSizeLimits);
        }
    }
}
