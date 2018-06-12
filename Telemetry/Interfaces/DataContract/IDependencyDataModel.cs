namespace CCLLC.Telemetry
{
    public interface IDependencyDataModel : IOperationalDataModel, IDataModel
    {        
        string resultCode { get; set; }
        string target { get; set; }
        string type { get; set; }
        string data { get; set; }
    }
}
