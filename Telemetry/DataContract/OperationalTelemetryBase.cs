using System;
using System.Collections.Generic;

namespace CCLCC.Telemetry.DataContract
{
    using CCLCC.Telemetry.Implementation;
    using Context;

    public abstract class OperationalTelemetryBase<TData> : TelemetryBase<TData>, IOperationalTelemetry where TData : IOperationalDataModel
    {
        public const int MaxNameLength = 1024;

        public TimeSpan Duration {
            get { return Utils.ValidateDuration(this.Data.duration); }
            set { this.Data.duration = value.ToString(); }
        }
        public string Id
        {
            get { return this.Data.id; }
            set { this.Data.id = value; }
        }
        public IDictionary<string, double> Metrics { get { return this.Data.measurements; } }
        public string Name
        {
            get { return this.Data.name; }
            set { this.Data.name = value; }
        }
        public IDictionary<string, string> Properties { get { return this.Data.properties; } }
        public bool? Success
        {
            get { return this.Data.success; }
            set { this.Data.success = value.Value; }
        }

        internal OperationalTelemetryBase(string telememtryName, ITelemetryContext context, TData data)
                    : base(telememtryName, context, data)
        {
        }

        public override void Sanitize()
        {
            this.Id = this.Id.TrimAndTruncate(MaxNameLength);
            this.Name = this.Name.TrimAndTruncate(MaxNameLength);
            this.Name = Utils.PopulateRequiredStringValue(this.Name);
            this.Properties.SanitizeProperties();
            this.Metrics.SanitizeMeasurements();
        }


    }
}
