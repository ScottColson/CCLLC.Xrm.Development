using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IMessageDataModel : IDataModel
    {
        string message { get; set; }
        SeverityLevel? severityLevel { get; set; }
    }
}
