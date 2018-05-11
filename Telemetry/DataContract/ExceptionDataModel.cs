using System;
using System.Collections.Generic;

namespace CCLCC.Telemetry.DataContract
{
    using Implementation;
    using System.Collections.Concurrent;

    public class ExceptionDataModel : IExceptionDataModel
    {
        public int ver { get; set; }
        public SeverityLevel? severityLevel { get; set; }
        public string problemId { get; set; }
        public IDictionary<string, double> measurements { get; set; }
        
        public IDictionary<string, string> properties { get; set; }
        public IList<IExceptionDetails> exceptions { get; set; }

        public string DataType { get { return "ExceptionData"; } }

        internal ExceptionDataModel()
        {
            ver = 2;
            exceptions = new List<IExceptionDetails>();
            problemId = "";
            properties = new ConcurrentDictionary<string, string>();
            measurements = new ConcurrentDictionary<string, double>();
        }

        T IDataModel.DeepClone<T>()
        {
            var other = new ExceptionDataModel();
            other.ver = this.ver;
            other.severityLevel = this.severityLevel;
            other.problemId = this.problemId;
            
            Utils.CopyDictionary(this.properties, other.properties);
            Utils.CopyDictionary(this.measurements, other.measurements);

            
            foreach (var e in this.exceptions)
            {
                other.exceptions.Add(e);
            }

            return other as T;
        }
    }
}
