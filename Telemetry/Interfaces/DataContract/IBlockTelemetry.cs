namespace CCLLC.Telemetry
{
    public interface IBlockTelemetry : ITelemetry, IOperationalTelemetry, IDataModelTelemetry<IBlockDataModel>
    {
    }
}
