using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    public interface IMessageDataModel : IDataModel
    {
        string message { get; set; }
        eSeverityLevel? severityLevel { get; set; }
    }
}
