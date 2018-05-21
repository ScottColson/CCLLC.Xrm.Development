
namespace CCLLC.Telemetry
{
    public interface IMessageTelemetry : ITelemetry, IDataModelTelemetry<IMessageDataModel>, ISupportProperties
    {
        eSeverityLevel? SeverityLevel { get; }
        string Message { get; }
    }
}
