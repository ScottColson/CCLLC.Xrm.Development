using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IStackFrame
    {
        int level { get; set; }
        string method { get; set; }
        string assembly { get; set; }
        string fileName { get; set; }
        int line { get; set; }
    }
}
