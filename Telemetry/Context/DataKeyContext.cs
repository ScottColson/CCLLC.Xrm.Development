using System.Collections.Generic;

namespace CCLCC.Telemetry.Context
{
    class DataKeyContext : IDataKeyContext
    {
        public string RecordId { get; set; }
        public string RecordType { get; set; }
        public string RecordSource { get; set; }
        public string AltKeyName { get; set; }
        public string AltKeyValue { get; set; }

        internal protected DataKeyContext() { }

        public void CopyTo(IDataKeyContext target)
        {
            target.RecordId = RecordId;
            target.RecordType = RecordType;
            target.RecordSource = RecordSource;
            target.AltKeyName = AltKeyName;
            target.AltKeyValue = AltKeyValue;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.DataRecordId, this.RecordId, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.DataRecordType, this.RecordType, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.DataRecordSource, this.RecordSource, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.DataRecordAltKeyName, this.AltKeyName, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.DataRecordAltKeyValue, this.AltKeyValue, keys.TagSizeLimits);
        }
    }
}
