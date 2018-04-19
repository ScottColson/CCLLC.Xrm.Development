using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmBase.Telemetry
{
    public interface IOperationTelemetryContext: ITelemetryContext, IDisposable 
    {       
        string OperationName { get; set; }
       
    }
}
