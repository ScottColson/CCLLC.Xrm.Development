using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
{
    public interface ITelemetryTransmitter : IDisposable
    {
        Uri EndpointAddress { get; set; }
        ITelemetrySerializer Serializer { get; }

        void Send(IEnumerable<ITelemetry> telemetryItems, TimeSpan timeout);

        Task SendAsync(IEnumerable<ITelemetry> telemetryItems, TimeSpan timeout);
    }
}
