namespace CCLLC.Telemetry
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
