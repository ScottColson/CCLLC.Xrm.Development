using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Sink
{
    using Interfaces;

    public class TelemetryTransmitter : ITelemetryTransmitter
    {
        public Uri EndpointAddress { get; set; }

        public ITelemetrySerializer Serializer { get; private set; }

        public TelemetryTransmitter(ITelemetrySerializer serializer)
        {
            this.Serializer = serializer;

        }
        public void Dispose()
        {
            this.Serializer = null;
            GC.SuppressFinalize(this);
        }

        public Task Send(IEnumerable<ITelemetry> telemetryItems, TimeSpan timeout)
        {
            if (telemetryItems == null) { return new Task(() =>{}); }
            if (telemetryItems.Count() <= 0) { return new Task(() => {}); }

            var content = Serializer.Serialize(telemetryItems);
            var transmission = new Transmission(this.EndpointAddress, content, this.Serializer.ContentType, this.Serializer.CompressionType, timeout);
            return transmission.SendAsync();
        }
    }
}
