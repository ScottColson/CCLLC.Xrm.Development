using System;
using Microsoft.Xrm.Sdk;
using CCLLC.Xrm.AppInsights;
using CCLLC.Telemetry;
using System.Net;
using System.IO;
using System.Text;
using System.Globalization;

namespace UsingXrmAppInsightsConnector
{
    public class ConnectedToAppInsightsPlugin : IPlugin
    {
        // The XrmAppInsightsConnector is threadsafe and stateless from the standpoint of the
        // plugin execution. It is setup as a plugin level variable to allow the Telemetry
        // channel to continue to run inbetween plugin executions.                
        IXrmAppInsightsConnector appInsightsConnector;

        string instrumentationKey;

        string unsecureConfig;
        string secureConfig;

        public ConnectedToAppInsightsPlugin(string unsecureConfig, string secureConfig)
        {
            this.unsecureConfig = unsecureConfig;
            this.secureConfig = secureConfig;

            // Create the XrmAppInsights connector when the plugin is first instantiated.
            this.appInsightsConnector = new XrmAppInsightsConnector();

            // Application Insights instrumentation key from the AI portal. This is hardcoded 
            // for this example but best practice is to pull this value in from configuration
            // data.
            this.instrumentationKey = "[EnterAIKey]"; 
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            
            // Generate a new insights client at each execution so that the properties captured in 
            // Application Insights reflect the current execution context.
            var insightsClient = appInsightsConnector.BuildClient(this.GetType().ToString(), executionContext, instrumentationKey);

            try
            {
                // Capture tracing information in Application Insights.
                insightsClient.Trace("Write an info message to Application Insights.");
                insightsClient.Trace(eMessageType.Error, "Write an error message to Application Insights.");

                // Track an event in Application Insights. Events are more visible than trace
                // messages so this can be good for documenting user intent based on message type 
                // and attribute values.
                insightsClient.TrackEvent("MyEventName");

                // Capture a web call depency in Application Insights.
                using(var op = insightsClient.StartDependencyOperation())
                {
                    try
                    {                        
                        using (WebClient client = new WebClient())
                        {
                            byte[] responseBytes = client.DownloadData("http://www.google.com");
                            string response = Encoding.UTF8.GetString(responseBytes);

                            // Capture the download size as a custom property 
                            op.Properties.Add("DownloadSize", response.Length.ToString());

                            // Tell Application Insights the operation completed successfully. If
                            // the code exits the "var op = " using block without calling this, 
                            // then the request will be logged as unsuccessful.
                            op.CompleteOperation(true);
                        }
                    }
                    catch (WebException exception)
                    {
                        string str = string.Empty;
                        if (exception.Response != null)
                        {
                            using (StreamReader reader =
                                new StreamReader(exception.Response.GetResponseStream()))
                            {
                                str = reader.ReadToEnd();
                            }
                            exception.Response.Close();
                        }
                        if (exception.Status == WebExceptionStatus.Timeout)
                        {
                            throw new InvalidPluginExecutionException(
                                "The timeout elapsed while attempting to issue the request.", exception);
                        }
                        throw new InvalidPluginExecutionException(String.Format(CultureInfo.InvariantCulture,
                            "A Web exception occurred while attempting to issue the request. {0}: {1}",
                            exception.Message, str), exception);
                    }                   
                }
            }
            catch(Exception ex)
            {
                // Capture exception details in Application Insights
                insightsClient.TrackException(ex);

                // Capture exception details in the plugin trace log.
                tracingService.Trace("Exception: {0}", ex.Message);

                throw;
            }
        }
    }
}
