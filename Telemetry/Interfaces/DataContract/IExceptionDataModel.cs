using System.Collections.Generic;

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
