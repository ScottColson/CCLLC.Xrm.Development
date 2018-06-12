namespace CCLLC.Telemetry
{
    public interface IMessageDataModel : IDataModel
    {
        string message { get; set; }
        eSeverityLevel? severityLevel { get; set; }
    }
}
