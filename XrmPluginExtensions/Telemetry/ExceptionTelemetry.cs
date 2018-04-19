using System;
using System.Collections.Generic;

namespace CCLCC.XrmBase.Telemetry
{
    public class ExceptionTelemetry : TelemetryBase, IExceptionTelemetry
    {
        public string ExceptionType { get; private set; }
        public Exception Exception { get; private set; }
        internal ExceptionTelemetry(Exception ex, IDictionary<string, string> properties) : base("Exception", properties, null)
        {
            this.ExceptionType = ex.GetType().ToString();
            this.Exception = ex;
            this.Message = ex.Message;
        }
    }
}
