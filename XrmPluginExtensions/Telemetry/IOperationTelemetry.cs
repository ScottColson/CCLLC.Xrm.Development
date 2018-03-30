using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmBase.Telemetry
{
    public interface IOperationTelemetry : ITelemetry
    {
        bool? Success { get; }
        TimeSpan Duration { get; }
    }
}
