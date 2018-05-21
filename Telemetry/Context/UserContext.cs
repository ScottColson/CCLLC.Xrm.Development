using System.Collections.Generic;

namespace CCLLC.Telemetry.Context
{
    public class UserContext : IUserContext
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string UserAgent { get; set; }
        public string AuthenticatedUserId { get; set; }

        internal protected UserContext() { }

        public void CopyTo(IUserContext target)
        {
            target.Id = Id;
            target.AccountId = AccountId;
            target.UserAgent = UserAgent;
            target.AuthenticatedUserId = AuthenticatedUserId;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.UserId, this.Id, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.UserAccountId, this.AccountId, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.UserAgent, this.UserAgent, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.UserAuthUserId, this.AuthenticatedUserId, keys.TagSizeLimits);
        }
    }
}
