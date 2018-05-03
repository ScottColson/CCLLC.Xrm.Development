using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{

    public interface IExceptionDetails
    {
        int id { get; set; }
        int outerId { get; set; }
        string typeName { get; set; }
        string message { get; set; }
        bool hasFullStack { get; set; }
        string stack { get; set; }
        IList<IStackFrame> parsedStack { get; set; }

    }
}
