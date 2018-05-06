using System.Collections.Generic;
using CCLCC.Telemetry.Implementation;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Context
{
    public class SessionContext : ISessionContext
    {
        public string Id { get; set; }
        public bool? IsFirst { get; set; }

        public void CopyTo(ISessionContext target)
        {
            Id = target.Id;
            IsFirst = target.IsFirst;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.SessionId, this.Id, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.SessionIsFirst, this.IsFirst);
        }
    }
}
