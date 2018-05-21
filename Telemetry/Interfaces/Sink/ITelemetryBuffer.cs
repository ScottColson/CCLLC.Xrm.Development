using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    public interface ITelemetryBuffer
    {
        /// <summary>
        /// Delegate that is raised when the buffer is full.
        /// </summary>
        Action OnFull { get;  set; }

        /// <summary>
        /// Gets or sets the buffer size that triggers transmission of the telemetry items
        /// </summary>
        int Capacity { get; set; }

        /// <summary>
        /// Gets or sets the maximum buffer backlog limit.
        /// </summary>
        int BacklogLimit { get; set; }

        /// <summary>
        /// Add an item to the buffer.
        /// </summary>
        /// <param name="item"></param>
        void Enqueue(ITelemetry item);

        /// <summary>
        /// Dequeue all existing telemetry items from the buffer.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITelemetry> Dequeue();

        int Length { get; }
    }
}
