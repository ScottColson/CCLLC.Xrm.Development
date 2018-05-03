using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Telemetry
{
    [System.Diagnostics.Tracing.EventData]
    public class StackFrame : IStackFrame
    {
        public int level { get; set; }
        public string method { get; set; }
        public string assembly { get; set; }
        public string fileName { get; set; }
        public int line { get; set; }
    }
}
