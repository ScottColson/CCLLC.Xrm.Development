
namespace CCLCC.XrmBase.Telemetry
{
    public interface ITraceTelemetry : ITelemetry
    {
        eSeverityLevel SeverityLevel { get; }
    }
}
