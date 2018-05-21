using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CCLLC.Telemetry.DataContract
{
    using Implementation;

    public class EventDataModel : IEventDataModel
    {
        public int ver { get; set; }
        public IDictionary<string, string> properties { get; set; }
        public string name { get; set; }
        public IDictionary<string, double> measurements { get; set; }

        public string DataType { get { return "EventData"; } }

        public EventDataModel()
        {
            ver = 2;
            this.name = "";
            properties = new ConcurrentDictionary<string, string>();
            measurements = new ConcurrentDictionary<string, double>();
        }

        public T DeepClone<T>() where T : class, IDataModel
        {
            var other = new EventDataModel();
            other.name = this.name;
            other.ver = this.ver;
            Utils.CopyDictionary<string>(this.properties, other.properties);
            Utils.CopyDictionary<double>(this.measurements, other.measurements);

            return other as T;
        }

    }
}
