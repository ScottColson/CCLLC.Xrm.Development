using System.Collections.Generic;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Telemetry
{
    using Context;
    using Implementation;

    public class MessageTelemetry : TelemetryBase<IMessageDataModel>, IMessageTelemetry
    {
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
               
        public MessageTelemetry(string message, SeverityLevel severityLevel, ITelemetryContext context, IMessageDataModel data, IDictionary<string,string> telemetryProperties = null) 
            : base("Message", context, data)
        {
            this.Message = message;
            this.SeverityLevel = severityLevel;
            if (telemetryProperties != null && telemetryProperties.Count > 0)
            {
                Utils.CopyDictionary<string>(telemetryProperties, this.Properties);
            }   
        }

        public override IDataModelTelemetry<IMessageDataModel> DeepClone()
        {
            throw new System.NotImplementedException();
        }

        public override void Sanitize()
        {
            throw new System.NotImplementedException();
        }

        public override void SerializeData(ITelemetrySerializer serializer, IJsonWriter writer)
        {
            writer.WriteProperty("ver", this.Data.ver);
            writer.WriteProperty("message", this.Message);
        }           
    }
}
