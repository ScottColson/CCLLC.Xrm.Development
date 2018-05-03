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


        public IDictionary<string, string> Properties { get; set; }
                         

        protected TelemetryClientBase(ITelemetryClient parentClient, IDictionary<string, string> contextProperties)
        {
            this.ParentClient = parentClient;
            this.Properties = contextProperties != null ? new Dictionary<string, string>(contextProperties) : new Dictionary<string, string>();
        }

        public virtual void Dispose()
        {
            Properties = null;
            ParentClient = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract void Initialize(ITelemetry telemetry);
        
        public IOperationalTelemetryClient<IBlockTelemetry> StartBlockOperation([CallerMemberName] string operationName = "", IDictionary<string, string> operationProperties = null, IDictionary<string, double> operationMetrics = null)
        {
            throw new NotImplementedException();
        }

        public IOperationalTelemetryClient<T> StartOperation<T>(T operationTelemetry) where T : IOperationalTelemetry
        {
            throw new NotImplementedException();
        }       

        public abstract void Track(ITelemetry telemetry);

       
       
             
           

       

       

       
                      
      

    }
}
