

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IRequestTelemetry : ITelemetry, IOperationalTelemetry, IDataModelTelemetry<IRequestDataModel>
    {
        string ResponseCode { get; set; }
        Uri Url { get; set; }
        string Source { get; set; }

    }
}
