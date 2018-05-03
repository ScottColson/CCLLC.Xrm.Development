using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CCLCC.Telemetry.Client
{
    using Interfaces;

    public abstract class ApplicationTelemetryClientBase : TelemetryClientBase, IApplicationTelemetryClient
    {
        public ITelemetrySink TelemetrySink { get; private set; }

        public string ClientName { get; private set; }
            
        public string ApplicationName { get; private set; }

        public string InstrumentationKey { get; private set; }

        public ApplicationTelemetryClientBase(string applicationName, ITelemetrySink telemetrySink, IDictionary<string,string> properties)
            : base(null,properties)
        {            
            this.TelemetrySink = telemetrySink;            
            this.ApplicationName = applicationName;
        }
             

        public override void Dispose()
        {                    
            TelemetrySink = null;
            base.Dispose();
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
