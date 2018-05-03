using System;
using System.Collections.Generic;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Telemetry
{
    using Context;
    using Implementation;

    public class ExceptionTelemetry : TelemetryBase<IExceptionDataModel>, IExceptionTelemetry, ISupportProperties, ISupportMetrics, ISupportSampling
    {
        public Exception Exception { get; private set; }

        public IDictionary<string, double> Metrics
        {
            get { return this.Data.measurements; }
        }

        public IDictionary<string, string> Properties
        {
            get { return this.Data.properties; }
        }

        public double? SamplingPercentage { get; set; }        
       
        public ExceptionTelemetry(Exception ex, ITelemetryContext context, IExceptionDataModel data, IDictionary<string, string> telemetryProperties = null, IDictionary<string,double> telemetryMetrics = null) : base("Exception", context, data)
        {           
            this.Exception = ex;

            if (telemetryProperties != null && telemetryProperties.Count > 0)
            {
                Utils.CopyDictionary<string>(telemetryProperties, this.Properties);
            }

            if (telemetryMetrics != null && telemetryMetrics.Count > 0)
            {
                Utils.CopyDictionary<double>(telemetryMetrics, this.Metrics);
            }
        }

        internal ExceptionTelemetry(IExceptionTelemetry source):this(source.Exception, source.Context.DeepClone(), source.Data.DeepClone<IExceptionDataModel>())
        {
            this.Sequence = source.Sequence;
            this.Timestamp = source.Timestamp;
            this.SamplingPercentage = source.SamplingPercentage;
            this.Exception = source.Exception;
        }

        public override IDataModelTelemetry<IExceptionDataModel> DeepClone()
        {
            return new ExceptionTelemetry(this);
        }

        public override void Sanitize()
        {
            throw new NotImplementedException();
        }

        public override void SerializeData(ITelemetrySerializer serializer, IJsonWriter writer)
        {
            writer.WriteProperty("ver", this.Data.ver);
            writer.WriteProperty("problemId", this.Data.problemId);
            writer.WriteProperty("properties", this.Data.properties);
            writer.WriteProperty("measurements", this.Data.measurements);
            writer.WritePropertyName("exceptions");
            {
                writer.WriteStartArray();

                serializer.SerializeExceptions(this.Data.exceptions, writer);

                writer.WriteEndArray();
            }

            if (this.Data.severityLevel.HasValue)
            {
                writer.WriteProperty("severityLevel", this.Data.severityLevel.Value.ToString());
            }
        }       
    }
}
