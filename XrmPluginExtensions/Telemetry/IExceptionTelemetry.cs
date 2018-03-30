using System;

namespace CCLCC.XrmBase.Telemetry
{
    public interface IExceptionTelemetry : ITelemetry
    {
        Exception Exception { get; }
    }
}
