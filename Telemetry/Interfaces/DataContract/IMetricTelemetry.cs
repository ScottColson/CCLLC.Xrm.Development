namespace CCLLC.Telemetry
{
    public interface IMetricTelemetry : ITelemetry, IDataModelTelemetry<IMetricDataModel>, ISupportProperties, ISupportSampling
    {
    }
}
