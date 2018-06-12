using System;
using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface IOperationalTelemetryClient<T>: ITelemetryClient, IDisposable where T : IOperationalTelemetry
    {
        /// <summary>
        /// Identifies the <see cref="ITelemetryClient"/> that is the parent of the current
        /// <see cref="IOperationalTelemetryClient{T}"/> instance.
        /// </summary>
        ITelemetryClient ParentClient { get; }

        /// <summary>
        /// Additional properties that will be applied to any <see cref="ITelemetry"/> items tracked
        /// within the context of the <see cref="IOperationalTelemetryClient{T}"/> instance.
        /// </summary>
        IDictionary<string, string> Properties { get; }

        /// <summary>
        /// Marks the operation as complete and captures the related <see cref="IOperationalTelemetry"/> 
        /// item. Will be called when the <see cref="IOperationalTelemetryClient{T}"/> is disposed 
        /// if not already called.
        /// </summary>
        /// <param name="success">Flag that sets the will document the success of the operation.</param>
        void CompleteOperation(bool? success);
    }
}
