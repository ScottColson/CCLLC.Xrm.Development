using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCLLC.Telemetry.Sink
{
    /// <summary>
    /// Serializes a collection of <see cref="ITelemetry"/> items and delivers 
    /// them to the telemetry service identified in the <see cref="EndpointAddress"/>. Depends
    /// on an instance of <see cref="ITelemetrySerializer"/> to serialize the telemetry to JSON.
    /// </summary>
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

        /// <summary>
        /// Asynchronously send an enumerable set of <see cref="ITelemetry"/> items to the 
        /// specified <see cref="EndpointAddress"/>.
        /// </summary>
        /// <param name="telemetryItems"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task SendAsync(IEnumerable<ITelemetry> telemetryItems, TimeSpan timeout)
        {
            if(this.EndpointAddress == null) { return new Task(() => { }); }
            if (telemetryItems == null) { return new Task(() =>{}); }
            if (telemetryItems.Count() <= 0) { return new Task(() => {}); }

            var content = Serializer.Serialize(telemetryItems);
            var transmission = BuildTransmission(content);
            return transmission.SendAsync(timeout);
        }

        /// <summary>
        /// Send an enumerable set of <see cref="ITelemetry"/> items to the 
        /// specified <see cref="EndpointAddress"/> synchronously. Use 
        /// <see cref="SendAsync(IEnumerable{ITelemetry}, TimeSpan)"/> when 
        /// supported by hosting application to minimize impact on application.
        /// </summary>
        /// <param name="telemetryItems"></param>
        /// <param name="timeout"></param>
        public void Send(IEnumerable<ITelemetry> telemetryItems, TimeSpan timeout)
        {
            if (this.EndpointAddress != null && telemetryItems != null && telemetryItems.Count() > 0)
            {
                var content = Serializer.Serialize(telemetryItems);
                var transmission = BuildTransmission(content);
                try
                {
                    transmission.Send(timeout);
                }
                catch
                { 
                    //swallow the error.
                }

            }
        }

        /// <summary>
        /// Internal factory method that builds out a new transmission for the specified content.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        protected ITransmission BuildTransmission(byte[] content)
        {
            return new Transmission(this.EndpointAddress, content, this.Serializer.ContentType, this.Serializer.CompressionType);
        }
    }
}
