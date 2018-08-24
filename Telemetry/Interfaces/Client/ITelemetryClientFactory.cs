using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface ITelemetryClientFactory
    {
        ITelemetryInitializerChain InitializerChain { get; }

        /// <summary>
        /// Configures and returns a new instance of <see cref="IComponentTelemetryClient"/>
        /// </summary>
        /// <param name="applicationName">The name of the application or component.</param>
        /// <param name="telemetrySink">The <see cref="ITelemetrySink"/> that services client.</param>
        /// <param name="contextProperties">An optional dictionary of properties that will become part of the telemetry context.</param> 
        IComponentTelemetryClient BuildClient(string applicationName, ITelemetrySink telemetrySink, IDictionary<string, string> contextProperties = null);
    }
}
