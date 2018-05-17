using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
{
    public interface IRequestDataModel : IOperationalDataModel, IDataModel
    {        
        string responseCode { get; set; }
        string source { get; set; }
        string url { get; set; }             

    }
}
