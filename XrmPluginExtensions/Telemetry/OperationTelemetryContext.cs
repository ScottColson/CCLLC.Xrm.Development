using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmBase.Telemetry
{
    public class OperationTelemetryContext : TelemetryContextBase, IOperationTelemetryContext
    {    
        private Stopwatch stopwatch;        
        private bool completed = false;
        private Dictionary<string, double> metrics;

        public string OperationName { get; set; }       

        internal OperationTelemetryContext(ITelemetryContext parentContext, string operationName, IDictionary<string, string> operationProperties, IDictionary<string, double> operationMetrics)
            : base(parentContext, (IDictionary<string,string>)parentContext.Properties)
        {     
            if(parentContext == null) { throw new ArgumentNullException("parentContext"); }
            this.metrics = new Dictionary<string, double>(operationMetrics);
            this.OperationName = operationName;
            if(operationProperties != null)
            {
                foreach(var p in operationProperties)
                {
                    AddProperty(p.Key, p.Value);
                }
            }

            TraceEvent(this.OperationName + "-start", null, metrics);
            
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }        

        public void CompleteOperation(bool? success)
        {            
            stopwatch.Stop();
            TraceOperation(this.OperationName, stopwatch.Elapsed, success, null, metrics);
            completed = true;           
        }

        public override void Dispose()
        {
            if (!completed)
            {
                CompleteOperation(null);
            }
            stopwatch = null;
            OperationName = null;
            base.Dispose();
        }

        public override void Track(ITelemetry telemetry)
        {
            ParentContext.Track(telemetry);
        }
    }
}
