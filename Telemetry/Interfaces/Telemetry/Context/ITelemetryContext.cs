using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface ITelemetryContext
    {
        string InstrumentationKey { get; }
        IDictionary<string, string> Properties { get; }
        ICloudContext Cloud { get; }
        IComponentContext Component { get; }  
        IDataContext Data { get; }
        IDeviceContext Device { get; }
        ILocationContext Location { get; }
        IOperationContext Operation { get; }
        ISessionContext Session { get; }
        IUserContext User { get; }
        IDictionary<string, string> SanitizedTags { get; }
        ITelemetryContext DeepClone();
       
    }
}
