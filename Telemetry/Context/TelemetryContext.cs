using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Context
{
    public class TelemetryContext : ITelemetryContext
    {
        private IComponentContext component;
        private IOperationContext operation;

        public IDictionary<string, string> Properties { get; private set; }
               
        public IOperationContext Operation { get { return LazyInitializer.EnsureInitialized(ref this.operation, () => new OperationContext()); } }

        public string InstrumentationKey { get; set; }

        public IDictionary<string, string> SanitizedTags => throw new NotImplementedException();

        public ICloudContext Cloud => throw new NotImplementedException();

        public IComponentContext Component => throw new NotImplementedException();

        public IDataContext Data => throw new NotImplementedException();
        
        public IDeviceContext Device => throw new NotImplementedException();

        public ILocationContext Location => throw new NotImplementedException();

        public ISessionContext Session => throw new NotImplementedException();

        public IUserContext User => throw new NotImplementedException();

        string ITelemetryContext.InstrumentationKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IInternalContext Internal => throw new NotImplementedException();

        public TelemetryContext():this(new ConcurrentDictionary<string, string>()) { }

        internal TelemetryContext(IDictionary<string, string> properties)
        {
            this.Properties = properties == null ? new ConcurrentDictionary<string, string>() : properties ;
        }

        public ITelemetryContext DeepClone()
        {
            throw new NotImplementedException();
        }

        public void CopyFrom(ITelemetryContext source)
        {
            this.InstrumentationKey = source.InstrumentationKey;

            source.Component?.CopyTo(this.Component);
            source.Data?.CopyTo(this.Data);
            source.Device?.CopyTo(this.Device);
            source.Cloud?.CopyTo(this.Cloud);
            source.Session?.CopyTo(this.Session);
            source.User?.CopyTo(this.User);
            source.Operation?.CopyTo(this.Operation);
            source.Location?.CopyTo(this.Location);
            source.Internal.CopyTo(this.Internal);
        }
    }
}
