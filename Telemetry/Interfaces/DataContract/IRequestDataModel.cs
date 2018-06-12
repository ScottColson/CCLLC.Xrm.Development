namespace CCLLC.Telemetry
{
    public interface IRequestDataModel : IOperationalDataModel, IDataModel
    {        
        string responseCode { get; set; }
        string source { get; set; }
        string url { get; set; }             

    }
}
