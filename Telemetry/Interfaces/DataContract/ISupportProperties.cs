using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface ISupportProperties
    {
        IDictionary<string, string> Properties { get; }
    }
}
