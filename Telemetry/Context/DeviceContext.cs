using System.Collections.Generic;

namespace CCLCC.Telemetry.Context
{
    public class DeviceContext : IDeviceContext
    {
        public string Id { get; set; }
        public string Model { get; set; }
        public string OemName { get; set; }
        public string OperatingSystem { get; set; }
        public string Type { get; set; }

        internal protected DeviceContext() { }

        public void CopyTo(IDeviceContext target)
        {
            Type = target.Type;
            Id = target.Id;
            OperatingSystem = target.OperatingSystem;
            OemName = target.OemName;
            Model = target.Model;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.DeviceType, this.Type, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.DeviceId, this.Id, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.DeviceOSVersion, this.OperatingSystem, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.DeviceOEMName, this.OemName, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.DeviceModel, this.Model, keys.TagSizeLimits);
        }
    }
}
