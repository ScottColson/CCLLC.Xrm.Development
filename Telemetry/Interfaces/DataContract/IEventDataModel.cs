using System.Collections.Generic;

namespace CCLLC.Telemetry
{
    public interface IEventDataModel : IDataModel
    {
        string name { get; set; }
        IDictionary<string, double> measurements { get; set; }
        


    }
}
