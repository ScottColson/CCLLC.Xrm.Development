
namespace CCLCC.Telemetry.Interfaces
{
    public interface IEventTelemetry : ITelemetry, IDataModelTelemetry<IEventDataModel>, ISupportProperties, ISupportMetrics, ISupportSampling
    {
        string Name { get; }
    }
}
