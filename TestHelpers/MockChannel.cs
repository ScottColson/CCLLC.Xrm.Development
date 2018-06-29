using CCLLC.Telemetry;
using System;


namespace TestHelpers
{
    public class MockChannel : ITelemetryChannel
    {
        public ITelemetryBuffer Buffer { get; private set; }

        public TimeSpan SendingInterval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan TransmissionTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Uri EndpointAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ITelemetryTransmitter Transmitter => throw new NotImplementedException();

        public MockChannel(ITelemetryBuffer buffer)
        {
            this.Buffer = buffer;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void Send(ITelemetry item)
        {
            if (item != null)
            {
                Buffer.Enqueue(item);
            }
        }
    }
}
