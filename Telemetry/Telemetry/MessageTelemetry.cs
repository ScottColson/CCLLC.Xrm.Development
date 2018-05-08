using System.Collections.Generic;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Telemetry
{
    using Context;
    using Implementation;

    public class MessageTelemetry : TelemetryBase<IMessageDataModel>, IMessageTelemetry
    {
        public const int MaxMessageLength = 32768;

        public SeverityLevel? SeverityLevel
        {
            get { return this.Data.severityLevel; }
            set { this.Data.severityLevel = value; }
        }

        public string Message
        {
            get { return this.Data.message; }
            set { this.Data.message = value; }
        }

        public IDictionary<string, string> Properties { get { return this.Data.properties; } }
               
        public MessageTelemetry(string message, SeverityLevel? severityLevel, ITelemetryContext context, IMessageDataModel data, IDictionary<string,string> telemetryProperties = null) 
            : base("Message", context, data)
        {
            this.Message = message;
            this.SeverityLevel = severityLevel;
            if (telemetryProperties != null && telemetryProperties.Count > 0)
            {
                Utils.CopyDictionary<string>(telemetryProperties, this.Properties);
            }   
        }

        private MessageTelemetry(IMessageTelemetry source) : this(source.Message, source.SeverityLevel, source.Context.DeepClone(), source.Data.DeepClone<IMessageDataModel>())
        {
            this.Sequence = source.Sequence;
            this.Timestamp = source.Timestamp;           
        }
        public override IDataModelTelemetry<IMessageDataModel> DeepClone()
        {
            return new MessageTelemetry(this);
        }

        public override void Sanitize()
        {
            this.Data.message = this.Data.message.TrimAndTruncate(MaxMessageLength);
            this.Data.message = Utils.PopulateRequiredStringValue(this.Data.message);
            this.Data.properties.SanitizeProperties();
        }

        public override void SerializeData(ITelemetrySerializer serializer, IJsonWriter writer)
        {
            writer.WriteProperty("ver", this.Data.ver);
            writer.WriteProperty("message", this.Message);
            if (this.Data.severityLevel.HasValue)
            {
                writer.WriteProperty("severityLevel", this.Data.severityLevel.Value.ToString());
            }
            if(this.Data.properties != null && this.Data.properties.Count> 0)
            {
                writer.WriteProperty("properties", this.Data.properties);
            }
            
        }           
    }
}
