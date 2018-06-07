using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Xrm.Sdk
{
    public interface IPluginWebRequest : IDisposable
    {
        ICredentials Credentials { get; set; }
        WebHeaderCollection Headers { get; set; }       
        TimeSpan Timeout { get; set; }
        IPluginWebResponse Get();
        IPluginWebResponse Post(byte[] data, string contentType, string contentEncoding = null);
    }
}
