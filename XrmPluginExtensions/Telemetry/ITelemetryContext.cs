using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CCLCC.XrmBase.Telemetry
{
    public interface ITelemetryContext
    {

        IReadOnlyDictionary<string, string> Properties { get; }
       
        ITelemetryContext ParentContext { get; }

        void AddProperty(string name, string value);
        
        IOperationTelemetryContext StartOperation([CallerMemberName]string operationName = "", IDictionary<string, string> operationProperties = null, IDictionary<string, double> operationMetrics = null);

        void Trace(string message, params object[] args);
        
        void Trace(eSeverityLevel severityLevel, string message, params object[] args);

        void TraceException(Exception exception);

        void TraceEvent(string eventName, IDictionary<string, string> eventProperties = null, IDictionary<string, double> eventMetrics = null);
        
        void TraceOperation(string operationName, TimeSpan duration, bool? success, IDictionary<string, string> operationProperties, IDictionary<string, double> operationMetrics);

        void Track(ITelemetry telemetry);
    }
}
