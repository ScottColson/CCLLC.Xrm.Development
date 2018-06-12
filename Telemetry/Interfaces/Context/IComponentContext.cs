using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface IComponentContext
    {
        string Name { get; set; }
        string Version { get; set; }

        void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys);

        void CopyTo(IComponentContext target);
    }
}
