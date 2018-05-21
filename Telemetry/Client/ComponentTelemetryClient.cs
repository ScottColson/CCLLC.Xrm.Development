using System;
using System.Collections.Generic;
using CCLLC.Telemetry.Implementation;

namespace CCLLC.Telemetry.Client
{  

    public class ComponentTelemetryClient : TelemetryClientBase, IComponentTelemetryClient
    {        
        public ITelemetryContext Context { get; private set; }

        public ITelemetrySink TelemetrySink { get; private set; }

        public ITelemetryInitializerChain Initializers { get; private set; }

        public string ApplicationName
        {
            get { return this.Context.Component.Name; }
            set { this.Context.Component.Name = value; }
        }

        public string InstrumentationKey
        {
            get { return this.Context.InstrumentationKey; }
            set { this.Context.InstrumentationKey = value; }
        }              

        internal protected ComponentTelemetryClient(string applicationName, ITelemetrySink telemetrySink, ITelemetryContext telemetryContext, ITelemetryInitializerChain initializers, IDictionary<string,string> contextProperties = null)
            : base(null)
        {            
            this.TelemetrySink = telemetrySink;
            this.Context = telemetryContext;
            this.Initializers = initializers;
            this.ApplicationName = applicationName;

            if(contextProperties != null && contextProperties.Count > 0)
            {
                Utils.CopyDictionary<string>(contextProperties, this.Context.Properties);
            }
        }             

        public override void Dispose()
        {                    
           this.TelemetrySink = null;
            this.Context = null;
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        public override void Initialize(ITelemetry telemetry)
        {
            //copy the context from the client.
            telemetry.Context.CopyFrom(this.Context);

            //copy any properties from the context if the telemetry support properties.
            var telemetryWithProperties = telemetry as ISupportProperties;
            if (telemetryWithProperties != null)
            {
                Utils.CopyDictionary(this.Context.Properties, telemetryWithProperties.Properties);
            }          

            //process telemetry through the initializer chain
            if(this.Initializers != null) { this.Initializers.Initialize(telemetry); }
                        
            if (telemetry.Timestamp == default(DateTimeOffset))
            {
                telemetry.Timestamp = DateTimeOffset.UtcNow;
            }

            // Application Insights requires SDK version in the Internal context in
            // version "name: version"
            if (string.IsNullOrEmpty(telemetry.Context.Internal.SdkVersion))
            {
                telemetry.Context.Internal.SdkVersion = "ccllc:0.1.1-100";
            }    
        }

        public override void Track(ITelemetry telemetryItem)
        {           
            if(TelemetrySink != null)
            {
                this.Initialize(telemetryItem);
                TelemetrySink.Process(telemetryItem);
            }
        }

       
    }
}
