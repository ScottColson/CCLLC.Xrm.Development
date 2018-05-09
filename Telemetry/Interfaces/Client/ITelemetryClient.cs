using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CCLCC.Telemetry
{
    public interface ITelemetryClient
    {
        

       
       
        ITelemetryClient ParentClient { get; }

        //IOperationalTelemetryClient<T> StartOperation<T>(string operationName, IDictionary<string, string> operationProperties = null, IDictionary<string, double> operationMetrics = null) where T : IOperationalTelemetry;
        
        //IOperationalTelemetryClient<T> StartOperation<T>(string operationName, string operationId, string parentOperationId = null, IDictionary<string, string> operationProperties = null, IDictionary<string, double> operationMetrics = null) where T : IOperationalTelemetry;

        IOperationalTelemetryClient<T> StartOperation<T>(T operationTelemetry) where T : IOperationalTelemetry;

        //void Trace(string message, params object[] args);
        
        //void Trace(SeverityLevel severityLevel, string message, params object[] args);

        //void TrackAvailability(IAvailabilityTelemetry telemetry);

        //void TrackDependency(IDependencyTelemetry telemetry);

        //void TrackException(IExceptionTelemetry telemetry);

        //void TrackException(Exception exception);

        //void TrackEvent(IEventTelemetry telemetry);

        //void TrackEvent(string eventName, IDictionary<string, string> eventProperties = null, IDictionary<string, double> eventMetrics = null);

        //void TrackMessage(IMessageTelemetry telemetry);

        //void TrackMetric(IMetricTelemetry telemetry);

        //void TrackOperation<T>(T telemetry) where T : IOperationalTelemetry;

        //void TrackOperation<T>(string operationName, TimeSpan duration, bool? success, IDictionary<string, string> operationProperties, IDictionary<string, double> operationMetrics) where T : IOperationalTelemetry;

        //void TrackPageView(IPageViewTelemetry telemetry);

        //void TrackRequest(IRequestTelemetry telemetry);      

        
        void Track(ITelemetry telemetry);

        
    }
}
