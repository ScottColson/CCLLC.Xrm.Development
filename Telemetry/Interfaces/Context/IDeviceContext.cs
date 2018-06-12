using System.Collections.Generic;

namespace CCLLC.Telemetry 
{
    public interface IDeviceContext 
    {
        string Id { get; set; }
        string Model { get; set; }
        string OemName { get; set; }
        string OperatingSystem { get; set; }
        string Type { get; set; }

        void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys);

        void CopyTo(IDeviceContext target);
    }
}
