
namespace CCLCC.Telemetry
{
    public interface IMessageTelemetry : ITelemetry, IDataModelTelemetry<IMessageDataModel>, ISupportProperties
    {
        SeverityLevel? SeverityLevel { get; }
        string Message { get; }
    }
}
