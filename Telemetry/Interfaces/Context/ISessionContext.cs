using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface ISessionContext 
    {
        string Id { get; set; }
        bool? IsFirst { get; set; }

        void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys);

        void CopyTo(ISessionContext target);
    }
}
