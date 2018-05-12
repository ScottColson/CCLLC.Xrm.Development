using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
{
    public interface IOperationalDataModel : IDataModel
    {
        string id { get; set; }
        string name { get; set; }
        IDictionary<string, double> measurements { get; set; }
        bool? success { get; set; }
        string duration { get; set; }
    }
}
