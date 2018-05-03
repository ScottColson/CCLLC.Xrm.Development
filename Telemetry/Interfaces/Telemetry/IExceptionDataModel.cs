using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IExceptionDataModel : IDataModel
    {
        IList<IExceptionDetails> exceptions { get; set; }
        SeverityLevel? severityLevel { get; set; }

        string problemId { get; set; }

        IDictionary<string, double> measurements { get; set; }
    }
}
