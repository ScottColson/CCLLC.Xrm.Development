using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    public interface IDependencyTelemetry : ITelemetry, IOperationalTelemetry, IDataModelTelemetry<IDependencyDataModel>
    {
        string DependencyType { get; set; }
        string Target { get; set; }
        string DependencyData { get; set; }
        string ResultCode { get; set; }
        
    }
}
