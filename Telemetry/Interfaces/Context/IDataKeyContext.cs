using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IDataKeyContext
    {
        string RecordType { get; set; }
        string RecordId { get; set; }

        void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys);

        void CopyTo(IDataKeyContext target);
    }
}
