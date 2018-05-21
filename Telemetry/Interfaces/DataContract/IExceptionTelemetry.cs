using System;

namespace CCLLC.Telemetry
{
    public interface IExceptionTelemetry :  ITelemetry, IDataModelTelemetry<IExceptionDataModel>, ISupportProperties, ISupportMetrics, ISupportSampling
    {
        Exception Exception { get; }
       
    }
}
