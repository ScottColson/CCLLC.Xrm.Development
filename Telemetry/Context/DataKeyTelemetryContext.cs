using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using CCLCC.Telemetry.Implementation;

namespace CCLCC.Telemetry.Context
{
    public class DataKeyTelemetryContext : TelemetryContext, ISupportDataKeyContext
    {
                     
        private IDataKeyContext data;             
       
        public IDataKeyContext Data { get { return LazyInitializer.EnsureInitialized(ref this.data, () => new DataKeyContext()); } }
             

        public DataKeyTelemetryContext() : this(new ConcurrentDictionary<string, string>()) { }

        internal DataKeyTelemetryContext(IDictionary<string, string> properties) : base(properties)
        {           
        }

        public override ITelemetryContext DeepClone()
        {
            var clone = new DataKeyTelemetryContext();
            clone.CopyFrom(this);
            if (this.Properties != null && this.Properties.Count > 0)
            {
                Utils.CopyDictionary<string>(this.Properties, clone.Properties);
            }
            return clone;
        }

        public override void CopyFrom(ITelemetryContext source)
        {
            base.CopyFrom(source);

            var asSupportsDataKeyContext = source as ISupportDataKeyContext;
            if (asSupportsDataKeyContext != null)
            {
                asSupportsDataKeyContext.Data?.CopyTo(this.Data);
            }
        }
         
        public override IDictionary<string, string> ToContextTags(IContextTagKeys keys)
        {
            var result = base.ToContextTags(keys);   
            this.data?.UpdateTags(result, keys);
            return result;
        }
    }
}
