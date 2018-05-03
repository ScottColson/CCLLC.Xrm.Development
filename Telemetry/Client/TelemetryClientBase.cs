using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Client
{    
    
    public abstract class TelemetryClientBase : ITelemetryClient
    {        
        public ITelemetryClient ParentClient { get; private set; }

        protected TelemetryClientBase(ITelemetryClient parentClient)
        {
            this.ParentClient = parentClient;            
        }

        public virtual void Dispose()
        {           
            ParentClient = null;
            GC.SuppressFinalize(this);
        }
        
        public abstract void Initialize(ITelemetry telemetry);

        public IOperationalTelemetryClient<T> StartOperation<T>(T operationTelemetry) where T : IOperationalTelemetry
        {
            return new OperationTelemetryClient<T>(this, operationTelemetry);
        }

        public abstract void Track(ITelemetry telemetry);

       
       
             
           

       

       

       
                      
      

    }
}
