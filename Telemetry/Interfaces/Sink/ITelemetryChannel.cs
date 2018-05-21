using System;
using System.Collections.Generic;

namespace CCLLC.Telemetry
{  
    public interface ITelemetryChannel : IDisposable
    {     
        ITelemetryBuffer Buffer { get; }       
        TimeSpan SendingInterval { get; set; }
        TimeSpan TransmissionTimeout { get; set; }
        Uri EndpointAddress { get; set; }

        ITelemetryTransmitter Transmitter { get;}
       
        /// <summary>
        /// Sends an instance of ITelemetry through the channel using the buffering strategy
        /// implemented for the channel.
        /// </summary>
        void Send(ITelemetry item);

        /// <summary>
        /// Transmit all unsent items in the buffer.
        /// </summary>
        void Flush();

       
    }
}
