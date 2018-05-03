using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using CCLCC.Telemetry.Interfaces;

namespace CCLCC.Telemetry.Telemetry.Context
{
    public class TelemetryContext : ITelemetryContext
    {
        private IComponentContext component;
        private IOperationContext operation;

        public IDictionary<string, string> Properties { get; private set; }
               
        public IOperationContext Operation { get { return LazyInitializer.EnsureInitialized(ref this.operation, () => new OperationContext()); } }

        public string InstrumentationKey => throw new NotImplementedException();

        public IDictionary<string, string> SanitizedTags => throw new NotImplementedException();

        public ICloudContext Cloud => throw new NotImplementedException();

        public IComponentContext Component => throw new NotImplementedException();

        public IDataContext Data => throw new NotImplementedException();
        
        public IDeviceContext Device => throw new NotImplementedException();

        public ILocationContext Location => throw new NotImplementedException();

        public ISessionContext Session => throw new NotImplementedException();

        public IUserContext User => throw new NotImplementedException();

        public TelemetryContext():this(new ConcurrentDictionary<string, string>()) { }

        internal TelemetryContext(IDictionary<string, string> properties)
        {
            this.Properties = properties == null ? new ConcurrentDictionary<string, string>() : properties ;
        }

        public ITelemetryContext DeepClone()
        {
            throw new NotImplementedException();
        }
    }
}
