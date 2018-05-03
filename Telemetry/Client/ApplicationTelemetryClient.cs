using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using CCLCC.Telemetry.Interfaces;
using CCLCC.Telemetry.Implementation;

namespace CCLCC.Telemetry.Client
{  

    public class ApplicationTelemetryClient : TelemetryClientBase, IApplicationTelemetryClient
    {        
        public ITelemetryContext Context { get; private set; }

        public ITelemetrySink TelemetrySink { get; private set; }         
                    
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

        public ApplicationTelemetryClient(string applicationName, ITelemetrySink telemetrySink, ITelemetryContext telemetryContext, IDictionary<string,string> contextProperties = null)
            : base(null)
        {            
            this.TelemetrySink = telemetrySink;
            this.Context = telemetryContext;
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
                        
            //copy any properties from the context if the telemetry support properties.
            var telemetryWithProperties = telemetry as ISupportProperties;
            if (telemetryWithProperties != null)
            {
                Utils.CopyDictionary(this.Context.Properties, telemetryWithProperties.Properties);
            }

            telemetry.Context.CopyFrom(this.Context);

            

            if (telemetry.Timestamp == default(DateTimeOffset))
            {
                telemetry.Timestamp = DateTimeOffset.UtcNow;
            }

            // Application Insights requires SDK version in the Internal context in
            // version "name: version"
            if (string.IsNullOrEmpty(telemetry.Context.Internal.SdkVersion))
            {                
                telemetry.Context.Internal.SdkVersion = "custom: 001.0000";
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
