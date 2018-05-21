using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLLC.Telemetry.Implementation;

namespace CCLLC.Telemetry.DataContract
{
    public class RequestDataModel : IRequestDataModel
    {
        public IDictionary<string, double> measurements { get; set; }
        public int ver { get; set; }
        public IDictionary<string, string> properties { get; set; }
        public string DataType { get { return "RequestData"; } }        
        public string id { get; set; }
        public string name { get; set; }        
        public string duration { get; set; }
        public bool? success { get; set; }      
        public string responseCode { get; set; }
        public string source { get; set; }
        public string url { get; set; }

        public RequestDataModel()
        {
            ver = 2;
            id = "";
            source = "";
            this.name = "";
            duration = "";
            responseCode = "";
            url = "";
            properties = new ConcurrentDictionary<string, string>();
            measurements = new ConcurrentDictionary<string, double>();
        }
        public T DeepClone<T>() where T : class, IDataModel
        {
            var other = new RequestDataModel();
            other.ver = this.ver;
            other.id = this.id;
            other.source = this.source;
            other.name = this.name;
            other.duration = this.duration;
            other.responseCode = this.responseCode;
            other.success = this.success;
            other.url = this.url;
            
            Utils.CopyDictionary(this.properties, other.properties);
            Utils.CopyDictionary(this.measurements, other.measurements);
            return other as T;
        }
    }
}
