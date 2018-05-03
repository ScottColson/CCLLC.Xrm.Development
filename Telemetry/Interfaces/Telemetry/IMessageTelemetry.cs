
namespace CCLCC.Telemetry.Interfaces
{
    public interface IMessageTelemetry : ITelemetry, IDataModelTelemetry<IMessageDataModel>, ISupportProperties
    {
        SeverityLevel? SeverityLevel { get; }
        string Message { get; }
    }
}
