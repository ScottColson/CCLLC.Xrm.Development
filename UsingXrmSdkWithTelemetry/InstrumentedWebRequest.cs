using System;
using CCLLC.Xrm.Sdk;

namespace UsingXrmSdkWithTelemetry
{
    public class InstrumentedWebRequest : InstrumentedPluginBase
    {
        public InstrumentedWebRequest(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
            //register against any message on Post Op.
            this.RegisterEventHandler(null, null, ePluginStage.PostOperation, EventHandler);
        }

        /// <summary>
        /// This event handler uses the <see cref="IPluginWebRequestFactory"/> surfaced in
        /// localContext to generate and execute a GET request to download data from Google. 
        /// The generated <see cref="IPluginWebRequest"/> provides basic error handling and 
        /// returns a <see cref="IPluginWebResponse"/> object with headers and content. Because 
        /// the plugin is based on <see cref="InstrumentedPluginBase"/>, the request will 
        /// automatically use dependency telementry tracking which captures the duration of 
        /// the request, and the statuscode in AppInsights.
        /// </summary>
        /// <param name="localContext"></param>
        private void EventHandler(ILocalPluginContext localContext)
        {
            using (var webRequest = localContext.CreateWebRequest(new Uri("http://www.google.com"), "Google"))
            {
                webRequest.Timeout = new TimeSpan(0, 0, 10); //change timeout from default 30 seconds to 10 seconds.

                var webResponse = webRequest.Get();  //get the response
                
                //log something to show it worked.
                localContext.Trace("Retrieved {0} bytes", webResponse.Content.Length);
            }                
        }
    }
}
