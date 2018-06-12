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
        private IEventLogger _eventLogger;

        public Uri EndpointAddress { get; set; }

        public ITelemetrySerializer Serializer { get; private set; }

        public TelemetryTransmitter(ITelemetrySerializer serializer, IEventLogger eventLogger)
        {
            _eventLogger = eventLogger;
            this.Serializer = serializer;
        }        

        public void Dispose()
        {
            this.Serializer = null;
            _eventLogger = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Asynchronously send an enumerable set of <see cref="ITelemetry"/> items to the 
        /// specified <see cref="EndpointAddress"/>.
        /// </summary>
        /// <param name="telemetryItems"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<IHttpWebResponseWrapper> SendAsync(IEnumerable<ITelemetry> telemetryItems, TimeSpan timeout)
        {
            try
            {
                if (this.EndpointAddress == null) { return new Task<IHttpWebResponseWrapper>(() => { return null; }); }
                if (telemetryItems == null) { return new Task<IHttpWebResponseWrapper>(() => { return null; }); }
                if (telemetryItems.Count() <= 0) { return new Task<IHttpWebResponseWrapper>(() => { return null; }); }

                var content = Serializer.Serialize(telemetryItems);
                var transmission = BuildTransmission(content);
                return transmission.SendAsync(timeout);
            }
            catch(Exception ex)
            {
                _eventLogger.FailedToSend(ex.Message);
                return new Task<IHttpWebResponseWrapper>(() => { return null; });
            }
        }

        /// <summary>
        /// Send an enumerable set of <see cref="ITelemetry"/> items to the 
        /// specified <see cref="EndpointAddress"/> synchronously. Use 
        /// <see cref="SendAsync(IEnumerable{ITelemetry}, TimeSpan)"/> when 
        /// supported by hosting application to minimize impact on application.
        /// </summary>
        /// <param name="telemetryItems"></param>
        /// <param name="timeout"></param>
        public IHttpWebResponseWrapper Send(IEnumerable<ITelemetry> telemetryItems, TimeSpan timeout)
        {
            if (this.EndpointAddress != null && telemetryItems != null && telemetryItems.Count() > 0)
            {
                try
                {
                    var content = Serializer.Serialize(telemetryItems);
                    var transmission = BuildTransmission(content);

                    return transmission.Send(timeout);
                }
                catch (Exception ex)
                {
                    _eventLogger.FailedToSend(ex.Message);                    
                }
            }

            return null;
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
