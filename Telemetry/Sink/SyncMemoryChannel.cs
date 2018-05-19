using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Sink
{
    public class SyncMemoryChannel : ITelemetryChannel
    {
        private int disposeCount = 0;
        private AutoResetEvent startRunnerEvent;
        private bool enabled = true;

        public ITelemetryBuffer Buffer { get; private set; }

        public ITelemetryTransmitter Transmitter { get; private set; }

        public TimeSpan SendingInterval { get; set; }

        public TimeSpan TransmissionTimeout { get; set; }

        public Uri EndpointAddress
        {
            get { return this.Transmitter.EndpointAddress; }
            set { this.Transmitter.EndpointAddress = value; }
        }

        public SyncMemoryChannel(ITelemetryBuffer buffer, ITelemetryTransmitter tranmitter)
        {
            this.Transmitter = tranmitter;
            
            this.Buffer = buffer;
            this.Buffer.OnFull = () => { this.Flush(); };  //connect Flush operation to Buffer.OnFull

            this.SendingInterval = new TimeSpan(0, 0, 15); //default sending interval is 15 seconds. 
            this.TransmissionTimeout = new TimeSpan(0, 0, 30); //default transmission timeout is 30 seconds.
            
            // Starting the Runner
            Task.Factory.StartNew(this.Runner, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Flush()
        {
            var items = Buffer.Dequeue();
            Transmitter.Send(items, this.TransmissionTimeout);
        }

        public void Send(ITelemetry item)
        {
            if (item != null)
            {
                Buffer.Enqueue(item);
            }
        }

        private void Dispose(bool disposing)
        {
            if (Interlocked.Increment(ref this.disposeCount) == 1)
            {
                // Stops the runner loop.
                this.enabled = false;

                if (this.startRunnerEvent != null)
                {
                    // Call Set to prevent waiting for the next interval in the runner.
                    try
                    {
                        this.startRunnerEvent.Set();
                    }
                    catch (ObjectDisposedException)
                    {
                        // We need to try catch the Set call in case the auto-reset event wait interval occurs between setting enabled
                        // to false and the call to Set then the auto-reset event will have already been disposed by the runner thread.
                    }
                }

                this.Flush();

                if (this.Transmitter != null)
                {
                    this.Transmitter.Dispose();
                }
            }
        }

        /// <summary>
        /// Flushes the in-memory buffer and sends the telemetry items in <see cref="sendingInterval"/> intervals or when 
        /// <see cref="startRunnerEvent" /> is set.
        /// </summary>
        private void Runner()
        {
            try
            {
                using (this.startRunnerEvent = new AutoResetEvent(false))
                {
                    while (this.enabled)
                    {
                        this.Flush();

                        // Waiting for the flush delay to elapse
                        this.startRunnerEvent.WaitOne(this.SendingInterval);
                    }
                }
            }
            finally
            {
            }
        }

        ~SyncMemoryChannel()
        {
            //force disposal process if the channel was not disposed of 
            //prior to dropping last reference and turning over to GC.
            this.Dispose();
        }

    }
}
