using System;
using System.Threading;

namespace CCLLC.Telemetry.Client
{        
    public class SequencePropertyInitializer : ITelemetryInitializer
    {
        
        private readonly string stablePrefix = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=') + ":";
        private long currentNumber;

        /// <summary>
        /// Populates <see cref="ITelemetry.Sequence"/> with unique ID and sequential number.
        /// </summary>
        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Sequence))
            {
                telemetry.Sequence = this.stablePrefix + Interlocked.Increment(ref this.currentNumber);
            }
        }
    }
}
