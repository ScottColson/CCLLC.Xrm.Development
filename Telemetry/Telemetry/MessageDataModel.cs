using System.Collections.Concurrent;
using System.Collections.Generic;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Telemetry
{
    using Implementation;


    public class MessageDataModel : IMessageDataModel
    {
        public int ver { get; set; }
        public IDictionary<string, string> properties { get; set; }
        public string message { get; set; }
        public SeverityLevel? severityLevel { get; set; }
        public MessageDataModel()
        {
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
    }
}
