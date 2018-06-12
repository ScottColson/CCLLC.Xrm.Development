using System.Collections.Generic;

namespace CCLLC.Telemetry
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
