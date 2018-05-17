using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
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
