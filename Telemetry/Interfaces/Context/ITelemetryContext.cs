using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface ITelemetryContext
    {
        string InstrumentationKey { get; set; }
        IDictionary<string, string> Properties { get; }
        ICloudContext Cloud { get; }
        IComponentContext Component { get; }  
        IDeviceContext Device { get; }
        IInternalContext Internal { get; }
        ILocationContext Location { get; }
        IOperationContext Operation { get; }
        ISessionContext Session { get; }
        IUserContext User { get; }
        
        ITelemetryContext DeepClone();

        /// <summary>
        /// Generates a new instance of the telemetry context.
        /// </summary>
        /// <returns></returns>
        ITelemetryContext BuildNew();

        void CopyFrom(ITelemetryContext source);

        IDictionary<string, string> ToContextTags(IContextTagKeys keys);
    }
}
