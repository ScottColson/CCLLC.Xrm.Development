using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IDataModel
    {
        int ver { get; set; }
        IDictionary<string,string> properties { get; set; }

        T DeepClone<T>() where T : class, IDataModel;
    }
}
