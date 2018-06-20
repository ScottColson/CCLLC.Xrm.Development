using System.Net;

namespace CCLLC.Xrm.Sdk
{
    public interface IPluginWebResponse
    {
        string Content { get; }
        WebHeaderCollection Headers { get; }
        int StatusCode { get; }
        string StatusDescription { get;}
        bool Success { get; }
    }
}
