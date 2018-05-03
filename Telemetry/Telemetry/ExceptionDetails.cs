using System.Collections.Generic;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Telemetry
{
    [System.Diagnostics.Tracing.EventData]
    public class ExceptionDetails : IExceptionDetails
    {
        public int id { get; set; }
        public int outerId { get; set; }
        public string typeName { get; set; }
        public string message { get; set; }
        public bool hasFullStack { get; set; }
        public string stack { get; set; }
        public IList<IStackFrame> parsedStack { get; set; }
    }
}
