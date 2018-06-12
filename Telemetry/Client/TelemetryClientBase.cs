using System;

namespace CCLLC.Telemetry.Client
{    
    
    public abstract class TelemetryClientBase : ITelemetryClient
    {  
        protected TelemetryClientBase()
        {                  
        }

        public virtual void Dispose()
        {                       
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
