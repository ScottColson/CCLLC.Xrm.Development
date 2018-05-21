using System.Collections.Concurrent;
using System.Collections.Generic;
using CCLLC.Telemetry.Implementation;
using CCLLC.Telemetry.DataContract;

namespace CCLLC.Telemetry.DataContract
{
    public class MessageDataModel : IMessageDataModel
    {
        public int ver { get; set; }
        public IDictionary<string, string> properties { get; set; }
        public string message { get; set; }
        public eSeverityLevel? severityLevel { get; set; }

        public string DataType { get; private set; }

        public MessageDataModel()
        {
            DataType = "MessageData";
            ver = 2;
            message = "";
            properties = new ConcurrentDictionary<string, string>();            
        }

        T IDataModel.DeepClone<T>()
        {
            var other = new MessageDataModel();
            other.ver = this.ver;
            other.message = this.message;
            other.severityLevel = this.severityLevel;

            Utils.CopyDictionary(this.properties, other.properties);
            return other as T;
        }

        public void Serialize(ITelemetrySerializer serializer, IJsonWriter writer)
        {
           writer.WriteProperty("ver", this.ver);
            writer.WriteProperty("message", this.message);
            if (this.severityLevel.HasValue)
            {
                writer.WriteProperty("severityLevel", this.severityLevel.Value.ToString());
            }
            if (this.properties != null && this.properties.Count > 0)
            {
                writer.WriteProperty("properties", this.properties);
            }
        }
    }
}
