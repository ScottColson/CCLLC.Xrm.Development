using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        void CopyFrom(ITelemetryContext source);

        IDictionary<string, string> ToContextTags(IContextTagKeys keys);
    }
}
