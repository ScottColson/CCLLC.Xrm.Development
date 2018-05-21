using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    public interface IDataModel
    {
        int ver { get; set; }
        IDictionary<string,string> properties { get; set; }

        string DataType { get; }
        T DeepClone<T>() where T : class, IDataModel;
       
        //void Serialize(ITelemetrySerializer serializer, IJsonWriter writer);
    }
}
