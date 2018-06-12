using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface ILocationContext 
    {
        string Ip { get; set; }

        void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys);

        void CopyTo(ILocationContext target);
    }
}
