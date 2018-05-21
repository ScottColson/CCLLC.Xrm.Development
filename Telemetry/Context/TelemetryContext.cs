using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using CCLLC.Telemetry.Implementation;

namespace CCLLC.Telemetry.Context
{
    public class TelemetryContext : ITelemetryContext
    {
        private readonly IDictionary<string, string> properties;

        private IComponentContext component;
        private IDeviceContext device;
        private ICloudContext cloud;
        private ISessionContext session;
        private IUserContext user;
        private IOperationContext operation;
        private ILocationContext location;
        private IInternalContext internalContext = new InternalContext();
        
        public ICloudContext Cloud { get { return LazyInitializer.EnsureInitialized(ref this.cloud, () => new CloudContext()); } }
        
        public IComponentContext Component { get { return LazyInitializer.EnsureInitialized(ref this.component, () => new ComponentContext()); } }
               
        public IDeviceContext Device { get { return LazyInitializer.EnsureInitialized(ref this.device, () => new DeviceContext()); } }

        public string InstrumentationKey { get; set; }

        public IInternalContext Internal { get { return this.internalContext; } }

        public ILocationContext Location { get { return LazyInitializer.EnsureInitialized(ref this.location, () => new LocationContext()); } }

        public IOperationContext Operation { get { return LazyInitializer.EnsureInitialized(ref this.operation, () => new OperationContext()); } }

        public IDictionary<string, string> Properties { get { return this.properties; } }

        public ISessionContext Session { get { return LazyInitializer.EnsureInitialized(ref this.session, () => new SessionContext()); } }

        public IUserContext User { get { return LazyInitializer.EnsureInitialized(ref this.user, () => new UserContext()); } }

        public TelemetryContext() : this(new ConcurrentDictionary<string, string>()) { }

        internal TelemetryContext(IDictionary<string, string> properties)
        {
            this.properties = properties == null ? new ConcurrentDictionary<string, string>() : properties;
        }

        public virtual ITelemetryContext DeepClone()
        {
            var clone = new TelemetryContext(new ConcurrentDictionary<string, string>());            
            clone.CopyFrom(this);
            if (this.Properties != null && this.Properties.Count > 0)
            {
                Utils.CopyDictionary<string>(this.Properties, clone.Properties);
            }
            return clone;
        }

        public virtual void CopyFrom(ITelemetryContext source)
        {
            this.InstrumentationKey = source.InstrumentationKey;

            source.Component?.CopyTo(this.Component);            
            source.Device?.CopyTo(this.Device);
            source.Cloud?.CopyTo(this.Cloud);
            source.Session?.CopyTo(this.Session);
            source.User?.CopyTo(this.User);
            source.Operation?.CopyTo(this.Operation);
            source.Location?.CopyTo(this.Location);
            source.Internal.CopyTo(this.Internal);                       
        }

        public virtual IDictionary<string, string> ToContextTags(IContextTagKeys keys)
        {
            var result = new Dictionary<string, string>();
            this.cloud?.UpdateTags(result, keys);
            this.component?.UpdateTags(result, keys);
            this.device?.UpdateTags(result, keys);
            this.Internal.UpdateTags(result, keys);            
            this.location?.UpdateTags(result, keys);
            this.operation?.UpdateTags(result, keys);
            this.session?.UpdateTags(result, keys);
            this.user?.UpdateTags(result, keys);   

            return result;
        }
    }
}
