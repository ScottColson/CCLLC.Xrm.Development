using System.Collections.Generic;
using CCLCC.Telemetry.Implementation;


namespace CCLCC.Telemetry.DataContract
{

    public class EventTelemetry : TelemetryBase<IEventDataModel>, IEventTelemetry
    {
        public const int MaxEventNameLength = 512;
        
        public string Name
        {
            get { return this.Data.name; }
            set { this.Data.name = value; }
        }        

        public IDictionary<string, double> Metrics { get { return this.Data.measurements; } }     

        public IDictionary<string, string> Properties { get { return this.Data.properties; } }

        public double? SamplingPercentage { get; set; }        

        public EventTelemetry(string name, ITelemetryContext context, IEventDataModel data, IDictionary<string,string> telemetryProperties = null, IDictionary<string,double> telemetryMetrics = null)
            : base("Event", context, data)
        {
            this.Name = name;
            if (telemetryProperties != null && telemetryProperties.Count > 0)
            {
                Utils.CopyDictionary<string>(telemetryProperties, this.Properties);
            }

            if (telemetryMetrics != null && telemetryMetrics.Count > 0)
            {
                Utils.CopyDictionary<double>(telemetryMetrics, this.Metrics);
            }
        }

        private EventTelemetry(IEventTelemetry source) : this(source.Name, source.Context.DeepClone(), source.Data.DeepClone<IEventDataModel>())
        {            
            this.Sequence = source.Sequence;
            this.Timestamp = source.Timestamp;
            this.SamplingPercentage = source.SamplingPercentage;
        }

        public override IDataModelTelemetry<IEventDataModel> DeepClone()
        {
            return new EventTelemetry(this);
        }

        public override void Sanitize()
        {
            this.Name = this.Name.TrimAndTruncate(MaxEventNameLength);
            this.Name = Utils.PopulateRequiredStringValue(this.Name);
            this.Properties.SanitizeProperties();
            this.Metrics.SanitizeMeasurements();
        }

        public override void SerializeData(ITelemetrySerializer serializer, IJsonWriter writer)
        {
            writer.WriteProperty("ver", this.Data.ver);
            writer.WriteProperty("name", this.Data.name);
            writer.WriteProperty("measurements", this.Data.measurements);
            writer.WriteProperty("properties", this.Data.properties);
        }
       
      
    }
}
