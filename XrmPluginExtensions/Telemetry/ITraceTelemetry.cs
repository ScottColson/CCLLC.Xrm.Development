
namespace CCLCC.XrmPluginExtensions.Telemetry
{
    public interface ITraceTelemetry : ITelemetry
    {
        eSeverityLevel SeverityLevel { get; }
    }
}
