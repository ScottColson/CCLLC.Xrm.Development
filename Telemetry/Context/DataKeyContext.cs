using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLCC.Telemetry.Implementation;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Context
{
    class DataKeyContext : IDataKeyContext
    {
        public string RecordId { get; set; }
        public string RecordType { get; set; }
        

        internal protected DataKeyContext() { }

        public void CopyTo(IDataKeyContext target)
        {
            RecordId = target.RecordId;
            RecordType = target.RecordType;
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.DataRecordId, this.RecordId, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.DataRecordType, this.RecordType, keys.TagSizeLimits);
        }
    }
}
