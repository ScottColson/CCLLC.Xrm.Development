using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
{
    public interface IEventDataModel : IDataModel
    {
        string name { get; set; }
        IDictionary<string, double> measurements { get; set; }
        


    }
}
