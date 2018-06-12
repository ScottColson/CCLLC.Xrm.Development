namespace CCLLC.Telemetry
{
    public interface ITelemetryClient
    {   
        /// <summary>
        /// Start a new operation and return the associated <see cref="IOperationalTelemetry"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operationTelemetry">The <see cref="IOperationalTelemetry"/> item that will be used
        /// to track the operation outcome.</param>
        /// <returns>A new disposable <see cref="IOperationalTelemetryClient{T}"/> instance.</returns>
        IOperationalTelemetryClient<T> StartOperation<T>(T operationTelemetry) where T : IOperationalTelemetry;
        
        /// <summary>
        /// Captures a <see cref="ITelemetry"/> item, initializes the telemetry context based on the client
        /// context, processes it through the stack of parent <see cref="ITelemetryClient"/> intances 
        /// and delivers to the <see cref="ITelemetrySink"/>.
        /// </summary>
        /// <param name="telemetry"></param>
        void Track(ITelemetry telemetry);
 
    }
}
