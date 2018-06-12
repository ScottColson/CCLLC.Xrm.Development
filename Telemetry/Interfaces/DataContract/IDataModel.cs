using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface IDataModel
    {
        int ver { get; set; }

        IDictionary<string,string> properties { get; set; }

        string DataType { get; }

        T DeepClone<T>() where T : class, IDataModel;       
        
    }
}
