namespace CCLLC.Telemetry
{
    public interface IDependencyTelemetry : ITelemetry, IOperationalTelemetry, IDataModelTelemetry<IDependencyDataModel>
    {
        string DependencyType { get; set; }
        string Target { get; set; }
        string DependencyData { get; set; }
        string ResultCode { get; set; }
        
    }
}
