using System;

namespace CCLLC.Telemetry
{
    public interface IRequestTelemetry : ITelemetry, IOperationalTelemetry, IDataModelTelemetry<IRequestDataModel>
    {
        string ResponseCode { get; set; }
        Uri Url { get; set; }
        string Source { get; set; }
        
    }
}
