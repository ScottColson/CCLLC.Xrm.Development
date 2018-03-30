using System;
using System.Collections.Generic;

namespace CCLCC.XrmBase.Telemetry
{
    public class ExceptionTelemetry : TelemetryBase, IExceptionTelemetry
    {
        public Exception Exception { get; private set; }
        internal ExceptionTelemetry(Exception ex, Dictionary<string, string> properties) : base("Exception", properties, null)
        {
            this.Exception = ex;
            this.Message = ex.Message;
        }
    }
}
