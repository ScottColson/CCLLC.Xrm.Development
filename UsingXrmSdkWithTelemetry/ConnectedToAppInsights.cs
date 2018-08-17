using System;
using CCLLC.Xrm.Sdk;  

namespace UsingXrmSdkWithTelemetry
{
    /// <summary>
    /// Sample plugin that inherits InstrumentedPluginBase which provides
    /// all of the enhancements included in PluginBase plus support for telmetry
    /// tracking on Application Insights. This example demonstrates the following:
    ///  - Using standard localContext tracing to write messages to Application Insights.
    ///  - Accessing advanced telemetry functions by accessing alternate interfaces of 
    ///    localContext.
    /// </summary>
    public class ConnectedToAppInsights : InstrumentedPluginBase
    {
        
        public ConnectedToAppInsights(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
            //register event handler to multiple message types for the contact entity.
            this.RegisterEventHandler("contact", MessageNames.Create, ePluginStage.PreOperation, MyCreateEventHandler);
            this.RegisterEventHandler("contact", MessageNames.Update, ePluginStage.PreOperation, MyUpdateEventHandler);          
        }

       
        private void MyCreateEventHandler(ILocalPluginContext localContext)
        {
            // This line will write to Plugin Trace Log as well as Application Insights
            localContext.Trace("Entered MyCreateEventHandler at {0}", DateTime.Now);  
        }

        private void MyUpdateEventHandler(ILocalPluginContext localContext)
        {
            // This line will write to Plugin Trace Log as well as Application Insights
            localContext.Trace("Entered MyUpdateEventHandler");
            
            //Track an event.
            localContext.TrackEvent("MyEventName");
            
            // To gain access to additional telemetry functions that are not part of the
            // ILocalPluginContext interface we need to cast into the interface that
            // provides the advanced telemetry support.             
            var asInstrumentedContext = localContext as ISupportContextInstrumentation;
            if(asInstrumentedContext != null)
            {
                // add special properties to the current telemetry context to capture an 
                // alternate key name and value. This is useful when using AppInsights 
                // across related components that don't share the same key. The value 
                // would normally come from a call to an integrated system but this example
                // just uses a random guid.
                asInstrumentedContext.SetAlternateDataKey("MySystemName", Guid.NewGuid().ToString());               
                
                // access the TelementryFactory and TelemetryClient directly to send a message
                // to AppInsights with a severity level. Using this method you can access anything 
                // that the Telemetry system offers even if that functionality is not surfaced in 
                // the localContext.
                var item = asInstrumentedContext.TelemetryFactory.BuildMessageTelemetry("This is a warning message.", CCLLC.Telemetry.eSeverityLevel.Warning);
                asInstrumentedContext.TelemetryClient.Track(item);
            }            
        }

    }
}
