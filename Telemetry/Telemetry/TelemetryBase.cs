﻿using System;
using System.Collections.Generic;
using System.Globalization;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Telemetry
{
    public abstract class TelemetryBase<TData> : ITelemetry, IDataModelTelemetry<TData> where TData : IDataModel
    {
        public string TelemetryName { get; private set; }
        public string BaseType { get; private set; }
        public TData Data { get; private set; }
        
        public DateTimeOffset Timestamp { get; set; }
        public string Sequence { get; set; }
        public string InstrumentationKey { get; set; }

        public ITelemetryContext Context { get; private set; }

        protected TelemetryBase(string telememtryName, ITelemetryContext context, TData data)
        {
            
            this.BaseType = data.GetType().ToString();
            this.Data = data;
            this.Context = context;
        }

        public abstract IDataModelTelemetry<TData> DeepClone();

        public abstract void Sanitize();

        public abstract void SerializeData(ITelemetrySerializer serializer, IJsonWriter writer);

       
        
    }
}
