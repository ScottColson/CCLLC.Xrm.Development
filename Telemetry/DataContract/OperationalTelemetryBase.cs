using System;
using System.Collections.Generic;

namespace CCLCC.Telemetry.DataContract
{
    using Context;

    public abstract class OperationalTelemetryBase<TData> : TelemetryBase<TData>, IOperationalTelemetry where TData : IDataModel
    {            
        public TimeSpan Duration { get; set; }
        public string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IDictionary<string,double> Metrics { get; private set; }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IDictionary<string,string> Properties { get; private set; }
        public bool? Success { get; set; }           

        internal OperationalTelemetryBase(string telememtryName, ITelemetryContext context, TData data)
            : base(telememtryName, context, data)
        {            
        }

       

      
    }
}
