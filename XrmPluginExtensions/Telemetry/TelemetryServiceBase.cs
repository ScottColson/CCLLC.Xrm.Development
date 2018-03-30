using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Telemetry
{
    public abstract class TelemetryServiceBase : ITelemetryService
    {
        private Dictionary<string, string> properties = new Dictionary<string, string>();

        public abstract bool WritesToPluginTracLog { get; }

        public virtual bool IsInitialized
        {
            get
            {
                return TelemetryProvider.IsInitialized;
            }
        }

        public virtual IReadOnlyDictionary<string, string> Properties
        {
            get { return properties; }
        }

        public virtual ITelemetryProvider TelemetryProvider { get; private set; } 
        
        public virtual ITracingService TracingService { get; private set; }

        protected TelemetryServiceBase(string pluginClassName, ITelemetryProvider telemetryProvider, ITracingService tracingService, IExecutionContext executionContext)
        {
            this.TracingService = tracingService;
            this.TelemetryProvider = telemetryProvider;

            properties.Add("crm-class-name", pluginClassName);
            properties.Add("crm-correlation-id", executionContext.CorrelationId.ToString());
            properties.Add("crm-organization-name", executionContext.OrganizationName);
            properties.Add("crm-entity-type", executionContext.PrimaryEntityName);
            properties.Add("crm-message-name", executionContext.MessageName);
            properties.Add("crm-execution-depth", executionContext.Depth.ToString());
            properties.Add("crm-isolation-mode", executionContext.IsolationMode.ToString());
            properties.Add("crm-initiating-user-id", executionContext.InitiatingUserId.ToString());
            properties.Add("crm-user-id", executionContext.UserId.ToString());
            properties.Add("crm-in-transaction",executionContext.IsInTransaction ? "true" : "false");
        }

        public void AddProperty(string name, string value)
        {
            if (!properties.ContainsKey(name))
            {
                properties.Add(name, value);
            }
        }

        public virtual void Dispose()
        {            
        }

        public virtual void TrackException(Exception exception)
        {
            var telemetry = new ExceptionTelemetry(exception, this.properties);
        }

        public virtual void TrackTrace(eSeverityLevel severityLevel, string message, params object[] args)
        {
            var telemetry = new TraceTelemetry(string.Format(message, args), severityLevel, this.properties);
            TelemetryProvider.Track(telemetry);
        }

        public virtual void TrackEvent(string eventName, IDictionary<string, string> eventProperties = null, IDictionary<string, double> eventMetrics = null)
        {

            IEventTelemetry telemetry;
            
            if(eventProperties == null || eventProperties.Count == 0)
            {
                telemetry = new EventTelemetry(eventName, this.properties, eventMetrics);
            }
            else
            {
                var props = new Dictionary<string, string>(this.properties);

                //copy over event properties but don't overwrite existing properties.
                foreach (var p in eventProperties)
                {
                    if (!props.ContainsKey(p.Key))
                    {
                        props.Add(p.Key, p.Value);
                    }
                }
                telemetry = new EventTelemetry(eventName, props, eventMetrics);
            }
            
            TelemetryProvider.Track(telemetry);
        }


        public virtual void TrackOperation(string operationName, TimeSpan duration, bool? success, IDictionary<string, string> operationProperties, IDictionary<string, double> operationMetrics)
        {
            IOperationTelemetry telemetry;

            if (operationProperties == null || operationProperties.Count == 0)
            {
                telemetry = new OperationTelemetry(operationName, duration, success, this.properties, operationMetrics);
            }
            else
            {
                var props = new Dictionary<string, string>(this.properties);

                //copy over operation properties but don't overwrite existing properties.
                foreach (var p in operationProperties)
                {
                    if (!props.ContainsKey(p.Key))
                    {
                        props.Add(p.Key, p.Value);
                    }
                }
                telemetry = new OperationTelemetry(operationName, duration, success, props, operationMetrics);
            }

            TelemetryProvider.Track(telemetry);
        }


        public abstract IOperationTelemetryInstance StartOperation(string operationName);
       
    }
}
