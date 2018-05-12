using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
{
    public interface IDependencyDataModel : IOperationalDataModel, IDataModel
    {        
        string resultCode { get; set; }
        string target { get; set; }
        string type { get; set; }
        string data { get; set; }
    }
}
