using System;
using System.Collections.Generic;

namespace CCLCC.Telemetry.Interfaces
{ 
    public interface IDataModelTelemetry<TData> where TData : IDataModel
    {        
        TData Data { get; }
        string BaseType { get; }
        IDataModelTelemetry<TData> DeepClone();
        void SerializeData(ITelemetrySerializer serializer, IJsonWriter writer);
    }
}
