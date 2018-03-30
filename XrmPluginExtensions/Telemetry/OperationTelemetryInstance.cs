using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmBase.Telemetry
{
    public class OperationTelemetryInstance : IOperationTelemetryInstance
    {    
        private Stopwatch stopwatch;
        private Dictionary<string, string> properties = new Dictionary<string, string>();
        private Dictionary<string, double> metrics = new Dictionary<string, double>();
        private bool completed = false;

        public ITelemetryService TelemetryService { get; private set; }

        public string OperationName { get; set; }       

        internal OperationTelemetryInstance(ITelemetryService telemetryService, string operationName)
        {
            this.TelemetryService = telemetryService;
            this.OperationName = operationName;
            stopwatch = new Stopwatch();
            stopwatch.Start();

            if (!string.IsNullOrEmpty(operationName))
            {
                telemetryService.TrackEvent(string.Format("Begin Operation: {0}", operationName));
            }
        }

        public void AddMetric(string name, double value)
        {
            metrics.Add(name, value);
        }

        public void AddProperty(string name, string value)
        {
            properties.Add(name, value);
        }

        public void CompleteOperation(bool? success)
        {            
            stopwatch.Stop();
            completed = true;
            
            this.TelemetryService.TrackOperation(this.OperationName, stopwatch.Elapsed, success, properties, metrics);      
        }

        public void Dispose()
        {
            if (!completed)
            {
                CompleteOperation(null);
            }            
        }
    }
}
