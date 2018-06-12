using System;

namespace CCLLC.Telemetry
{
    public interface ITelemetrySink
    {
        /// <summary>
        /// Delegate called by the <see cref="ITelemetrySink"/> when it needs configuration 
        /// information. Only called if the <see cref="ITelemetrySink"/> has not already been
        /// successfully configured. The delegate must return a Boolean indicating whether 
        /// configuration is successful.
        /// </summary>
        Func<bool> OnConfigure { get; set; }

        /// <summary>
        /// Flag indicating that the sink has already been configured. Set by the 
        /// return value of <see cref="OnConfigure"/> when called.
        /// </summary>
        bool IsConfigured { get; }

        /// <summary>
        /// The <see cref="ITelemetryChannel"/> that manages buffering, serialization and 
        /// transmission of telemetry on recurring transmission intervals.
        /// </summary>
        ITelemetryChannel Channel { get; }

        /// <summary>
        /// Defined chain of <see cref="ITelemetryProcessor"/> objects that process
        /// the a <see cref="ITelemetry"/> item prior to buffering it for transmission.
        /// </summary>
        ITelemetryProcessChain ProcessChain { get; }

        /// <summary>
        /// Processes a <see cref="ITelemetry"/> through the <see cref="ITelemetryProcessChain"/>
        /// and then buffers it for transmission during the next sending interval.
        /// </summary>
        /// <param name="telemetryItem"></param>
        void Process(ITelemetry telemetryItem);
       
    }
}
