using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Xrm.Sdk.Utilities
{
    public class PluginHttpWebResponse : IPluginWebResponse
    {
        public string Content { get; private set; }
        public WebHeaderCollection Headers { get; private set; }
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }
        public bool Success { get; private set; }
        
        internal PluginHttpWebResponse(HttpWebResponse response)
        {
            if (response != null)
            {
                this.Headers = new WebHeaderCollection();
                foreach (var key in response.Headers.AllKeys)
                {
                    this.Headers.Add(key, response.Headers[key]);
                }

                this.StatusCode = (int)response.StatusCode;
                this.StatusDescription = response.StatusDescription;
                this.Success = (this.StatusCode >= 200 && this.StatusCode < 300);

                using (StreamReader content = new StreamReader(response.GetResponseStream()))
                {
                    this.Content = content.ReadToEnd();
                }
            }
        }
    }
}
