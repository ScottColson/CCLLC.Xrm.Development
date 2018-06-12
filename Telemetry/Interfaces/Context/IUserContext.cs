using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface IUserContext 
    {
        string Id { get; set; }
        string AccountId { get; set; }
        string UserAgent { get; set; }
        string AuthenticatedUserId { get; set; }
        void CopyTo(IUserContext target);
        void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys);
       
    }
}
