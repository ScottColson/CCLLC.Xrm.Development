using System;
using System.Globalization;
using System.IO;
using System.Net;
using CCLLC.Telemetry;


namespace CCLLC.Xrm.Sdk.Utilities
{
    public class PluginHttpWebRequest : IPluginWebRequest
    {
        private HttpWebRequest _webRequest;
        private ITelemetryFactory _telemetryFactory = null;
        private ITelemetryClient _telemetryClient = null;
        private string _dependencyName = null;

        public byte[] Content { get; set; }

        public string ContentType
        {
            get { return _webRequest.ContentType; }
            set { _webRequest.ContentType = value; }
        }      
        
        public ICredentials Credentials
        {
            get { return _webRequest.Credentials; }
            set { _webRequest.Credentials = value; }
        }
      
        public WebHeaderCollection Headers
        {
            get { return _webRequest.Headers; }
            set { _webRequest.Headers = value; }
        }

        public string Method
        {
            get { return _webRequest.Method; }
            set { _webRequest.Method = value; }
        }

        public TimeSpan Timeout { get; set; }
        
        internal PluginHttpWebRequest(Uri address, string dependencyName = null, ITelemetryFactory telemetryFactory = null, ITelemetryClient telementryClient = null)
        {
            _dependencyName = dependencyName;
            _telemetryClient = telementryClient;
            _telemetryFactory = telemetryFactory;
            _webRequest = (HttpWebRequest)WebRequest.Create(address);
            this.Timeout = new TimeSpan(0, 0, 30); //Default timeout is 30 seconds.
        }
            
        public virtual IPluginWebResponse Send()
        {
            IDependencyTelemetry dependencyTelemetry = null;
            IOperationalTelemetryClient<IDependencyTelemetry> dependencyClient = null;

            if (_telemetryClient != null && _telemetryFactory != null)
            {
                dependencyTelemetry = _telemetryFactory.BuildDependencyTelemetry(
                    "Web", 
                    _webRequest.RequestUri.ToString(), 
                    _dependencyName != null ? _dependencyName : "PluginWebRequest",
                    null);
                dependencyClient = _telemetryClient.StartOperation<IDependencyTelemetry>(dependencyTelemetry);
            }

            try
            {
                if (this.Content == null) { this.Content = new byte[0]; }

                _webRequest.Timeout = (int)this.Timeout.TotalMilliseconds;
                using (Stream requestStream = _webRequest.GetRequestStream())
                {
                    requestStream.Write(this.Content, 0, this.Content.Length);
                }

                using (HttpWebResponse response = _webRequest.GetResponse() as HttpWebResponse)
                {
                    var pluginWebResponse = new PluginHttpWebResponse(response);
                    if (dependencyTelemetry != null)
                    {
                        dependencyTelemetry.ResultCode = pluginWebResponse.StatusCode.ToString();
                        dependencyTelemetry.Success = pluginWebResponse.Success;                       
                    }

                    if(dependencyClient != null) { dependencyClient.Dispose(); }

                    return pluginWebResponse;
                }

            }
            catch (WebException ex)
            {
                string str = string.Empty;
                if (ex.Response != null)
                {
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        str = reader.ReadToEnd();
                    }
                    ex.Response.Close();
                }
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    throw new Exception("Plugin Web Request Timeout occurred.", ex);
                }
                throw new Exception(String.Format(CultureInfo.InvariantCulture,
                    "A Web exception occurred while attempting to issue the request. {0}: {1}",
                    ex.Message, str), ex);
            }
            finally
            {
                if(dependencyClient != null) { dependencyClient.Dispose(); }
            }

        }
      
    }
}
