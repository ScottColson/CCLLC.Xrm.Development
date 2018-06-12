using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface IDataKeyContext
    {
        string RecordType { get; set; }
        string RecordId { get; set; }
        string RecordSource { get; set; }

        string AltKeyName { get; set; }
        string AltKeyValue { get; set; }

        void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys);

        void CopyTo(IDataKeyContext target);
    }
}
