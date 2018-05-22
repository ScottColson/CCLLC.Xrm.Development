using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLLC.Telemetry.Implementation;

namespace CCLLC.Telemetry.DataContract
{
    public class RequestTelemetry : OperationalTelemetryBase<IRequestDataModel>, IRequestTelemetry, ISupportSampling
    {
        public const int MaxUrlLength = 2048;
        

        public string ResponseCode
        {
            get { return this.Data.responseCode; }
            set { this.Data.responseCode = value; }
        }

        public Uri Url
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Data.url))
                {
                    return null;
                }

                return new Uri(this.Data.url, UriKind.RelativeOrAbsolute);
            }

            set
            {
                if (value != null)
                {
                    this.Data.url = value.ToString();
                }
                else
                {
                    this.Data.url = null;
                }
            }
        }
        public string Source
        {
            get { return this.Data.source; }
            set { this.Data.source = value; }
        }
        public double? SamplingPercentage { get; set; }

        public RequestTelemetry(string source, Uri url, ITelemetryContext context, IRequestDataModel dataModel, IDictionary<string, string> telemetryProperties = null, IDictionary<string, double> telemetryMetrics = null)
            : base("Request", context, dataModel)
        {            
            this.Url = url;
            this.Source = source;
            if (telemetryProperties != null && telemetryProperties.Count > 0)
            {
                Utils.CopyDictionary<string>(telemetryProperties, this.Properties);
            }

            if (telemetryMetrics != null && telemetryMetrics.Count > 0)
            {
                Utils.CopyDictionary<double>(telemetryMetrics, this.Metrics);
            }
        }

        private RequestTelemetry(IRequestTelemetry source) : this(source.Source, source.Url, source.Context.DeepClone(), source.Data.DeepClone<IRequestDataModel>())
        {            
            this.Sequence = source.Sequence;
            this.Timestamp = source.Timestamp;            
        }

        public override IDataModelTelemetry<IRequestDataModel> DeepClone()
        {
            return new RequestTelemetry(this);
        }

        public override void Sanitize()
        {
            base.Sanitize();

            // Set for backward compatibility:
            this.Data.id = this.Data.id.TrimAndTruncate(MaxNameLength);
            this.Data.id = Utils.PopulateRequiredStringValue(this.Data.id);

            if (Url != null)
            {
                string url = Url.ToString();

                if (url.Length > MaxUrlLength)
                {
                    url = url.Substring(0, MaxUrlLength);

                    //if the truncated Url is not valid then replace it with null which effectively removes
                    //it from the list of properties.
                    Uri temp;
                    if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out temp) == true)
                    {
                        Url = temp;
                    }
                }
            }

            if (string.IsNullOrEmpty(this.ResponseCode))
            {
                this.ResponseCode = "200";
                this.Success = true;
            }

        }

        public override void SerializeData(ITelemetrySerializer serializer, IJsonWriter writer)
        {
            writer.WriteProperty("ver", this.Data.ver);
            writer.WriteProperty("id", this.Data.id);
            writer.WriteProperty("source", this.Data.source);
            writer.WriteProperty("name", this.Data.name);
            writer.WriteProperty("duration", this.Duration);
            writer.WriteProperty("success", this.Data.success);
            writer.WriteProperty("responseCode", this.Data.responseCode);
            writer.WriteProperty("url", this.Data.url);
            writer.WriteProperty("measurements", this.Data.measurements);
            writer.WriteProperty("properties", this.Data.properties);
        }

        public override IDictionary<string, string> GetTaggedData()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("ver", this.Data.ver.ToString());
            dict.Add("id", this.Data.id);
            dict.Add("source", this.Data.source);
            dict.Add("name", this.Data.name);
            dict.Add("duration", this.Data.duration);
            if (this.Data.success.HasValue)
            {
                dict.Add("success", this.Data.success.ToString());
            }
            dict.Add("responseCode", this.Data.responseCode);
            dict.Add("url", this.Data.url);            
            return dict;
        }
    }
}
