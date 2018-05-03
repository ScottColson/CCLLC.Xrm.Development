using System;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IExceptionTelemetry :  ITelemetry, IDataModelTelemetry<IExceptionDataModel>, ISupportProperties, ISupportMetrics, ISupportSampling
    {
        Exception Exception { get; }
       
    }
}
