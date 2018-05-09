using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
{
    public interface IRequestDataModel : IDataModel
    {
        string id { get; set; }
        string source { get; set; }
        string name { get; set; }
        string duration { get; set; }
        string responseCode { get; set; }
        bool success { get; set; }
        string url { get; set; }
        IDictionary<string, double> measurements { get; set; }

    }
}
