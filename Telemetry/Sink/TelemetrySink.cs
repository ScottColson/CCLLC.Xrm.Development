using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Sink
{
    public class TelemetrySink : ITelemetrySink
    {
        public Action OnConfigure { get; set; }

        public bool IsInitialized { get { return !string.IsNullOrEmpty(this.Channel.Transmitter.EndpointAddress.OriginalString); } }

        public ITelemetryChannel Channel { get; private set; }

        public ITelemetryProcessChain ProcessChain { get; private set; }

        public TelemetrySink(ITelemetryChannel channel, ITelemetryProcessChain processChain)
        {
            if (channel == null) throw new ArgumentNullException("channel");
            this.Channel = channel;

            if (processChain == null) throw new ArgumentNullException("processChain");
            this.ProcessChain = processChain;

            
        }

        public void Process(ITelemetry telemetryItem)
        {
            if (!this.IsInitialized && OnConfigure != null)
            {
                OnConfigure();
            }

            if (this.IsInitialized)
            {
                this.ProcessChain.Process(telemetryItem);
                this.Channel.Send(telemetryItem);
            }
            
        }
    }
}
