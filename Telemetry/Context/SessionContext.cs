using System.Collections.Generic;


namespace CCLCC.Telemetry.Context
{
    public class SessionContext : ISessionContext
    {
        public string Id { get; set; }
        public bool? IsFirst { get; set; }

        public void CopyTo(ISessionContext target)
        {
            target.Id = Id;
            target.IsFirst = IsFirst;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.SessionId, this.Id, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.SessionIsFirst, this.IsFirst);
        }
    }
}
