using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry.Sink
{
    public class TelemetrySink : ITelemetrySink
    {
        private static object _configureLock = new object();
        private Func<bool> _onConfigure;
        public Func<bool> OnConfigure
        {
            get { return _onConfigure; }
            set
            {
                lock (_configureLock)
                {
                    _onConfigure = value;
                }
            }
        }


        public bool IsConfigured { get; private set; } 

        public ITelemetryChannel Channel { get; private set; }

        public ITelemetryProcessChain ProcessChain { get; private set; }

        public TelemetrySink(ITelemetryChannel channel, ITelemetryProcessChain processChain) : this(channel, processChain, false) { }

        public TelemetrySink(ITelemetryChannel channel, ITelemetryProcessChain processChain, bool isConfigured = false)
        {
            if (channel == null) throw new ArgumentNullException("channel");
            this.Channel = channel;

            if (processChain == null) throw new ArgumentNullException("processChain");
            this.ProcessChain = processChain;

            this.IsConfigured = isConfigured;            
        }

        public void Process(ITelemetry telemetryItem)
        {
            if (!this.IsConfigured && OnConfigure != null)
            {
                this.IsConfigured = OnConfigure();
            }

            if (this.IsConfigured)
            {
                this.ProcessChain.Process(telemetryItem);
                this.Channel.Send(telemetryItem);
            }
            
        }
    }
}
