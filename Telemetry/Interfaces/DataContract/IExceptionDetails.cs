using System.Collections.Generic;

namespace CCLLC.Telemetry
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
