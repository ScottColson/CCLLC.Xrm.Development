using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    public interface ITelemetryTransmitter : IDisposable
    {
        Uri EndpointAddress { get; set; }
        ITelemetrySerializer Serializer { get; }

        IHttpWebResponseWrapper Send(IEnumerable<ITelemetry> telemetryItems, TimeSpan timeout);

        Task<IHttpWebResponseWrapper> SendAsync(IEnumerable<ITelemetry> telemetryItems, TimeSpan timeout);
    }
}
