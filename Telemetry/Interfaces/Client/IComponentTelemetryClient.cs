using System;

namespace CCLLC.Telemetry
{
    public interface IComponentTelemetryClient : ITelemetryClient, IDisposable
    {
        string ApplicationName { get; set; }
        string InstrumentationKey { get; set; }
        ITelemetrySink TelemetrySink { get; }   
        ITelemetryContext Context { get; }
        ITelemetryInitializerChain Initializers { get; }

        /// <summary>
        /// Transmit all buffered <see cref="ITelemetry"/> items in the <see cref="ITelemetrySink"/>.
        /// </summary>
        void Flush();
    }
}
