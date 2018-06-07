using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Xrm.Sdk
{
    public interface IPluginWebRequest
    {
        byte[] Content { get; set; }
        string ContentType { get; set; }  
        ICredentials Credentials { get; set; }
        WebHeaderCollection Headers { get; set; }
        string Method { get; set; }
        TimeSpan Timeout { get; set; }
        IPluginWebResponse Send();
    }
}
