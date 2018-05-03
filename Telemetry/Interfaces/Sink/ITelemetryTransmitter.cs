using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface ITelemetryTransmitter : IDisposable
    {
        Uri EndpointAddress { get; set; }
        ITelemetrySerializer Serializer { get; }

        Task Send(IEnumerable<ITelemetry> telemetryItems, TimeSpan timeout);
    }
}
