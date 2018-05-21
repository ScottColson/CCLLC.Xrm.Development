using CCLLC.Telemetry.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.Telemetry.DataContract
{
    public class DependencyTelemetry : OperationalTelemetryBase<IDependencyDataModel>, IDependencyTelemetry, ISupportSampling
    {
        public const int MaxResultCodeLength = 1024;
        public const int MaxDependencyTypeLength = 1024;
        public const int MaxDataLength = 8 * 1024;

        public string DependencyType
        {
            get { return this.Data.type; }
            set { this.Data.type = value; }
        }
        public string Target
        {
            get { return this.Data.target; }
            set { this.Data.target = value; }
        }
        public double? SamplingPercentage { get; set; }

        /// <summary>
        /// Gets or sets data associated with the current dependency instance. Command name/statement for SQL dependency, URL for http dependency.
        /// </summary>
        public string DependencyData
        {
            get { return this.Data.data; }
            set { this.Data.data = value; }
        }
        public string ResultCode
        {
            get { return this.Data.resultCode; }
            set { this.Data.resultCode = value; }
        }

        

        public DependencyTelemetry(string dependencyTypeName, string target, string dependencyName, string dependencyData, ITelemetryContext context, IDependencyDataModel dataModel, IDictionary<string, string> telemetryProperties = null, IDictionary<string, double> telemetryMetrics = null)
            :base ("RemoteDependency", context, dataModel)
        {
            this.DependencyType = dependencyTypeName;
            this.Name = dependencyName;
            this.Target = target;
            this.DependencyData = dependencyData;

            if (telemetryProperties != null && telemetryProperties.Count > 0)
            {
                Utils.CopyDictionary<string>(telemetryProperties, this.Properties);
            }

            if (telemetryMetrics != null && telemetryMetrics.Count > 0)
            {
                Utils.CopyDictionary<double>(telemetryMetrics, this.Metrics);
            }
        }

        private DependencyTelemetry(DependencyTelemetry source) : this(source.DependencyType, source.Target, source.Name,source.DependencyData, source.Context.DeepClone(), source.Data.DeepClone<IDependencyDataModel>())
        {            
            this.Sequence = source.Sequence;
            this.Timestamp = source.Timestamp;
            this.SamplingPercentage = source.SamplingPercentage;            
        }

        public override IDataModelTelemetry<IDependencyDataModel> DeepClone()
        {
            return new DependencyTelemetry(this);            
        }

        public override void Sanitize()
        {
            base.Sanitize();
            this.ResultCode = this.ResultCode.TrimAndTruncate(MaxResultCodeLength);
            this.DependencyType = this.DependencyType.TrimAndTruncate(MaxDependencyTypeLength);
            this.DependencyData = this.DependencyData.TrimAndTruncate(MaxDataLength);
        }

        public override void SerializeData(ITelemetrySerializer serializer, IJsonWriter writer)
        {
            writer.WriteProperty("ver", this.Data.ver);
            writer.WriteProperty("name", this.Data.name);
            writer.WriteProperty("id", this.Data.id);
            writer.WriteProperty("data", this.Data.data);
            writer.WriteProperty("duration", this.Data.duration);
            writer.WriteProperty("resultCode", this.Data.resultCode);
            writer.WriteProperty("success", this.Data.success);
            writer.WriteProperty("type", this.Data.type);
            writer.WriteProperty("target", this.Data.target);

            writer.WriteProperty("properties", this.Data.properties);
            writer.WriteProperty("measurements", this.Data.measurements);
        }
    }
}
