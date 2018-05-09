using System;

namespace CCLCC.Telemetry
{
    public interface IExceptionTelemetry :  ITelemetry, IDataModelTelemetry<IExceptionDataModel>, ISupportProperties, ISupportMetrics, ISupportSampling
    {
        Exception Exception { get; }
       
    }
}
