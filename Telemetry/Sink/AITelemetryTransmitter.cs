using System;

namespace CCLLC.Telemetry.Sink
{

    /// <summary>
    /// Serializes a collection of <see cref="ITelemetry"/> items using the and delivers 
    /// them to the telemetry service identified in the <see cref="EndpointAddress"/>. Defaults
    /// to send to Application Insights service endpoint. Depends on an instance of 
    /// <see cref="ITelemetrySerializer"/> to serialize the telemetry to JSON.
    /// </summary>
    public class AITelemetryTransmitter : TelemetryTransmitter
    {
        public AITelemetryTransmitter(ITelemetrySerializer serializer) : base(serializer)
        {
            this.EndpointAddress = new Uri(AIConstants.TelemetryServiceEndpoint);
        }
    }
}
