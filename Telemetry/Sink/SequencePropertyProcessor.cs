using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CCLLC.Telemetry.Sink
{
    public class SequencePropertyProcessor : ITelemetryProcessor
    {
        private readonly string stablePrefix = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=') + ":";
        private long currentNumber;

        /// <summary>
        /// Populates <see cref="ITelemetry.Sequence"/> with unique ID and sequential number.
        /// </summary>       
        public void Process(ITelemetry telemetryItem)
        {
            if (string.IsNullOrEmpty(telemetryItem.Sequence))
            {
                telemetryItem.Sequence = this.stablePrefix + Interlocked.Increment(ref this.currentNumber);
            }
        }
    }
}
