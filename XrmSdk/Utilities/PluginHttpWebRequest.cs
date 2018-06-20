using System;
using System.Globalization;
using System.IO;
using System.Net;
using CCLLC.Telemetry;


namespace CCLLC.Xrm.Sdk.Utilities
{
    public class PluginHttpWebRequest : IPluginWebRequest
    {        
        internal const string ContentEncodingHeader = "Content-Encoding";

        private Uri _address = null;
        private ITelemetryFactory _telemetryFactory = null;
        private ITelemetryClient _telemetryClient = null;
        private string _dependencyName = null;

        
        public ICredentials Credentials { get; set; }
        
      
        public WebHeaderCollection Headers { get; set; }
        

        public TimeSpan Timeout { get; set; }
        

        internal PluginHttpWebRequest(Uri address, string dependencyName = null, ITelemetryFactory telemetryFactory = null, ITelemetryClient telementryClient = null)
        {
            if (address == null) { throw new ArgumentNullException("address cannot be null."); }

            _dependencyName = dependencyName;
            _telemetryClient = telementryClient;
            _telemetryFactory = telemetryFactory;
            _address = address;

            this.Headers = new WebHeaderCollection();
            this.Timeout = new TimeSpan(0, 0, 30); //Default timeout is 30 seconds.
        }
            
        public void Dispose()
        {
            _address = null;
            _dependencyName = null;
            _telemetryClient = null;
            _telemetryFactory = null;
            this.Credentials = null;
            this.Headers = null;
            
            GC.SuppressFinalize(this);
        }

        public virtual IPluginWebResponse Get()
        {
            IDependencyTelemetry dependencyTelemetry = null;
            IOperationalTelemetryClient<IDependencyTelemetry> dependencyClient = null;

            if (_telemetryClient != null && _telemetryFactory != null)
            {
                dependencyTelemetry = _telemetryFactory.BuildDependencyTelemetry(
                    "Web",
                    _address.ToString(),
                    _dependencyName != null ? _dependencyName : "PluginWebRequest",
                    null);
                dependencyClient = _telemetryClient.StartOperation<IDependencyTelemetry>(dependencyTelemetry);
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_address);
                request.Method = "GET";
                if (this.Credentials != null)
                {
                    request.Credentials = this.Credentials;
                }
                request.Headers = this.Headers;                
                request.Timeout = (int)this.Timeout.TotalMilliseconds;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    var pluginWebResponse = new PluginHttpWebResponse(response);
                    if (dependencyTelemetry != null)
                    {
                        dependencyTelemetry.ResultCode = pluginWebResponse.StatusCode.ToString();                        
                    }
                    response.Close();

                    //signals completion of the request operation for telemetry tracking.
                    if (dependencyClient != null)
                    {
                        dependencyClient.CompleteOperation(pluginWebResponse.Success);
                        dependencyClient.Dispose();
                    }

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
                if (dependencyClient != null) { dependencyClient.Dispose(); }
            }

        }
      

        public virtual IPluginWebResponse Post(byte[] data, string contentType, string contentEncoding = null)             
        {
            IDependencyTelemetry dependencyTelemetry = null;
            IOperationalTelemetryClient<IDependencyTelemetry> dependencyClient = null;            
            
            if (_telemetryClient != null && _telemetryFactory != null)
            {
                dependencyTelemetry = _telemetryFactory.BuildDependencyTelemetry(
                    "Web",
                    _address.ToString(),
                    _dependencyName != null ? _dependencyName : "PluginWebRequest",
                    null);
                dependencyClient = _telemetryClient.StartOperation<IDependencyTelemetry>(dependencyTelemetry);
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_address);
                request.Method = "POST";
                if(this.Credentials != null)
                {
                    request.Credentials = this.Credentials;
                }
                request.Headers = this.Headers;
                request.ContentType = contentType;

                if (!string.IsNullOrEmpty(contentEncoding))
                {
                    request.Headers[ContentEncodingHeader] = contentEncoding;
                }

                request.Timeout = (int)this.Timeout.TotalMilliseconds;
                                             
                if (data.Length > 0)
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(data, 0, data.Length);
                    }
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    var pluginWebResponse = new PluginHttpWebResponse(response);
                    if (dependencyTelemetry != null)
                    {
                        dependencyTelemetry.ResultCode = pluginWebResponse.StatusCode.ToString();                       
                    }
                    response.Close();

                    //signals completion of the request operation for telemetry tracking.
                    if (dependencyClient != null)
                    {
                        dependencyClient.CompleteOperation(pluginWebResponse.Success);
                        dependencyClient.Dispose();
                    }


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
                if (dependencyClient != null) { dependencyClient.Dispose(); }
            }

        }
    }
}
