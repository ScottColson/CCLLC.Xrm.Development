using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IComponentContext
    {
        string Name { get; set; }
        string Version { get; set; }

        void UpdateTags(IDictionary<string, string> tags);

        void CopyTo(IComponentContext target);
    }
}
