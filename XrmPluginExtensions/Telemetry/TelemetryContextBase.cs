using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmBase.Telemetry
{
    public abstract class TelemetryContextBase : ITelemetryContext
    {
        private Dictionary<string, string> properties;
        public IReadOnlyDictionary<string, string> Properties
        {
            get { return properties; }
        }       

        public ITelemetryContext ParentContext { get; private set; }
        
        protected TelemetryContextBase(ITelemetryContext parentContext, IDictionary<string, string> contextProperties)
        {
            this.ParentContext = parentContext;
            this.properties = contextProperties != null ? new Dictionary<string, string>(contextProperties) : new Dictionary<string, string>();
        }

        public void AddProperty(string key, string value)
        {
            if (!properties.ContainsKey(key))
            {
                properties.Add(key, value);
            }
        }

        public IOperationTelemetryContext StartOperation([CallerMemberName]string operationName = "", IDictionary<string, string> operationProperties = null, IDictionary<string, double> operationMetrics = null)
        {
            return new OperationTelemetryContext(this, operationName, operationProperties, operationMetrics);
        }

        public virtual void Trace(string message, params object[] args)
        {
            this.Trace(eSeverityLevel.Information, message, args);
        }

        public virtual void Trace(eSeverityLevel severityLevel, string message, params object[] args)
        {
            Track(new TraceTelemetry(string.Format(message, args), severityLevel, this.properties));
        }
       
        public virtual void TraceEvent(string eventName, IDictionary<string, string> eventProperties = null, IDictionary<string, double> eventMetrics = null)
        {
            if (eventProperties == null || eventProperties.Count == 0)
            {
                Track(new EventTelemetry(eventName, this.properties, eventMetrics));
            }
            else
            {
                //merge event properties with context properties but don't overwrite context properties.
                var props = new Dictionary<string, string>(this.properties);
                foreach (var p in eventProperties)
                {
                    if (!props.ContainsKey(p.Key))
                    {
                        props.Add(p.Key, p.Value);
                    }
                }
                Track(new EventTelemetry(eventName, props, eventMetrics));
            }

        }

        public virtual void TraceException(Exception exception)
        {
            Track(new ExceptionTelemetry(exception, this.properties));
        }


        public virtual void TraceOperation(string operationName, TimeSpan duration, bool? success, IDictionary<string, string> operationProperties, IDictionary<string, double> operationMetrics)
        {         
            if (operationProperties == null || operationProperties.Count == 0)
            {
                Track(new OperationTelemetry(operationName, duration, success, this.properties, operationMetrics));
            }
            else
            {
                //merge operation properties with context properties but don't overwrite context properties.
                var props = new Dictionary<string, string>(this.properties);
                foreach (var p in operationProperties)
                {
                    if (!props.ContainsKey(p.Key))
                    {
                        props.Add(p.Key, p.Value);
                    }
                }
                Track(new OperationTelemetry(operationName, duration, success, props, operationMetrics));
            }
        }

        public abstract void Track(ITelemetry telemetry);

        public virtual void Dispose()
        {
            properties = null;
            ParentContext = null;
        }
    }
}
