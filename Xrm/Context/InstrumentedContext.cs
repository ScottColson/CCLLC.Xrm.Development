using System;
using CCLLC.Core;
using CCLLC.Telemetry;
using Microsoft.Xrm.Sdk;

namespace CCLLC.Xrm.Sdk.Context
{
    public abstract class InstrumentedContext<E> : LocalContext<E>, ISupportContextInstrumentation where E : Entity
    {
        public IComponentTelemetryClient TelemetryClient { get; private set; }

        private ITelemetryFactory _telemetryFactory;
        public ITelemetryFactory TelemetryFactory
        {
            get
            {
                if (_telemetryFactory == null)
                {
                    _telemetryFactory = this.Container.Resolve<ITelemetryFactory>();
                }
                return _telemetryFactory;
            }
        }

        public InstrumentedContext(IExecutionContext executionContext, IIocContainer container, IComponentTelemetryClient telemetryClient) : base(executionContext, container)
        {
            if(telemetryClient == null) { throw new ArgumentNullException("telemetryClient"); }
            this.TelemetryClient = telemetryClient;
        }

        public override IPluginWebRequest CreateWebRequest(Uri address, string dependencyName = null)
        {
            return WebRequestFactory.BuildPluginWebRequest(address, dependencyName,  this.TelemetryFactory, this.TelemetryClient);
        }

        public override void Dispose()
        {            
            if (this.TelemetryClient != null)
            {
                if (this.TelemetryClient.TelemetrySink != null)
                {
                    this.TelemetryClient.TelemetrySink.OnConfigure = null;
                }
                this.TelemetryClient.Dispose();
            }

            base.Dispose();
        }
               
        public virtual void SetAlternateDataKey(string name, string value)
        {
            if (this.TelemetryClient != null)
            {
                var asDataContext = this.TelemetryClient.Context as ISupportDataKeyContext;
                if (asDataContext != null)
                {
                    asDataContext.Data.AltKeyName = name;
                    asDataContext.Data.AltKeyValue = value;
                }
            }
        }

        public override void Trace(eMessageType type, string message, params object[] args)
        {
            base.Trace(type, message, args);
            if (!string.IsNullOrEmpty(message))
            {
                if (this.TelemetryClient != null && this.TelemetryFactory != null)
                {
                    var level = eSeverityLevel.Information;
                    if (type == eMessageType.Warning)
                    {
                        level = eSeverityLevel.Warning;
                    }
                    else if (type == eMessageType.Error)
                    {
                        level = eSeverityLevel.Error;
                    }

                    var msgTelemetry = this.TelemetryFactory.BuildMessageTelemetry(string.Format(message, args), level);
                    this.TelemetryClient.Track(msgTelemetry);
                }
            }
        }

        public virtual void TrackEvent(string name)
        {
            if(this.TelemetryFactory != null && this.TelemetryClient != null && !string.IsNullOrEmpty(name))
            {
                this.TelemetryClient.Track(this.TelemetryFactory.BuildEventTelemetry(name));
            }
        }
    }
}
