using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    public interface IOperationContext
    {
        string Id { get; set; }
        string ParentId { get; set; }
        string Name { get; set; }
        string CorrelationVector { get; set; }

        void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys);

        void CopyTo(IOperationContext target);
    }
}
