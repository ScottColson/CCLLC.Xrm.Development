using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    public interface IExceptionDataModel : IDataModel
    {
        IList<IExceptionDetails> exceptions { get; set; }
        eSeverityLevel? severityLevel { get; set; }

        string problemId { get; set; }

        IDictionary<string, double> measurements { get; set; }
    }
}
