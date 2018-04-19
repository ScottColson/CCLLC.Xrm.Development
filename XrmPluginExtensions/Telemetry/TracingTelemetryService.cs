using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Telemetry
{
    public class TracingTelemetryService : TelemetryService
    { 
       
       

        public TracingTelemetryService(string pluginClassName, ITelemetryProvider telemetryProvider, IExecutionContext executionContext) 
            : base(pluginClassName,telemetryProvider, executionContext)
        {           
        }

       
        


    }
}
