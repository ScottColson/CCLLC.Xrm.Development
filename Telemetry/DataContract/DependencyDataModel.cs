using CCLCC.Telemetry.Implementation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.DataContract
{
    public class DependencyDataModel : IDependencyDataModel
    {
        public string target { get; set; }
        public IDictionary<string, double> measurements { get; set; }
        public int ver { get; set; }
        public IDictionary<string, string> properties { get; set; }

        public string DataType { get { return "RemoteDependencyData"; } }


        public string id { get; set; }
        public string name { get; set; }
        public string resultCode { get; set; }
        public string duration { get; set; }
        public bool? success { get; set; }
        public string type { get; set; }
        public string data { get; set; }
        bool? IOperationalDataModel.success { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DependencyDataModel()
        {
            this.ver = 2;
            this.name = "";
            this.id = "";
            this.resultCode = "";
            this.duration = "";
            this.success = true;

            this.target = "";
            this.type = "";
            this.properties = new ConcurrentDictionary<string, string>();
            this.measurements = new ConcurrentDictionary<string, double>();
        }

        public T DeepClone<T>() where T : class, IDataModel
        {
            var other = new DependencyDataModel();
            other.ver = this.ver;
            other.name = this.name;
            other.id = this.id;
            other.resultCode = this.resultCode;
            other.duration = this.duration;
            other.success = this.success;
            other.data = this.data;
            other.target = this.target;
            other.type = this.type;
           
            Utils.CopyDictionary(this.properties, other.properties);
            Utils.CopyDictionary(this.measurements, other.measurements);
            return other as T;
        }
    }
}
