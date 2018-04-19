using System;
using System.Collections.Generic;

namespace CCLCC.XrmBase.Telemetry
{
    public interface ITelemetryService : ITelemetryContext, IDisposable
    {
        IReadOnlyDictionary<string, string> Properties { get; }
     
        ITelemetryProvider TelemetryProvider { get; }

        void Track(ITelemetry telemetry);
    }
}
