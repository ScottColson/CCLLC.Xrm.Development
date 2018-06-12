namespace CCLLC.Telemetry
{
    public interface IHttpWebResponseWrapper
    {        
        string Content { get; set; }        
        int StatusCode { get; set; }
        string RetryAfterHeader { get; set; }
        string StatusDescription { get; set; }
    }
}
