using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Client
{
    using Interfaces;
   

    public class OperationTelemetryClient<T> : TelemetryClientBase, IOperationalTelemetryClient<T> where T : IOperationalTelemetry
    {    
        private Stopwatch stopwatch;        
        private bool completed = false;
        private T telemetryItem;

        public string OperationName { get; set; }       

        internal OperationTelemetryClient(ITelemetryClient parentClient, T telemetryItem)
            : base(parentClient, null)
        {
            this.telemetryItem = telemetryItem;                      
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }        

        public void CompleteOperation(bool? success)
        {
            stopwatch.Stop();
            telemetryItem.Duration = stopwatch.Elapsed;
            if (success.HasValue)
            {
                telemetryItem.Success = success;
            }
            Track(telemetryItem as ITelemetry);
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
            this.Initialize(telemetry);
            ParentClient.Track(telemetry);
        }

        public override void Initialize(ITelemetry telemetry)
        {
            throw new NotImplementedException();
        }
    }
}
