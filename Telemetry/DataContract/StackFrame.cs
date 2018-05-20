
namespace CCLCC.Telemetry.DataContract
{
    public partial class StackFrame : IStackFrame
    {
        public int level { get; set; }
        public string method { get; set; }
        public string assembly { get; set; }
        public string fileName { get; set; }
        public int line { get; set; }
    }
}
