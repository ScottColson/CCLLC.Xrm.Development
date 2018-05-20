using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLCC.Core;
using CCLCC.Telemetry;
using Microsoft.Xrm.Sdk;

namespace CCLCC.Xrm.Sdk.Context
{
    public abstract class InstrumentedContext<E> : LocalContext<E>, ISupportContextInstrumentation where E : Entity
    {
        public IComponentTelemetryClient TelemetryClient { get; private set; }

        private ITelemetryFactory telemetryFactory;
        public ITelemetryFactory TelemetryFactory
        {
            get
            {
                if (telemetryFactory == null)
                {
                    telemetryFactory = this.Container.Resolve<ITelemetryFactory>();
                }
                return telemetryFactory;
            }
        }

        public InstrumentedContext(IExecutionContext executionContext, IIocContainer container, IComponentTelemetryClient telemetryClient) : base(executionContext, container)
        {
            if(telemetryClient == null) { throw new ArgumentNullException("telemetryClient"); }
            this.TelemetryClient = telemetryClient;
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

        public override void Trace(eMessageType type, string message, params object[] args)
        {
            base.Trace(type, message, args);
            if (!string.IsNullOrEmpty(message))
            {
                if (this.TelemetryClient != null && this.TelemetryFactory != null)
                {
                    var level = eSeverityLevel.Information;
                    if(type == eMessageType.Warning)
                    {
                        level = eSeverityLevel.Warning;
                    }
                    else if(type == eMessageType.Error)
                    {
                        level = eSeverityLevel.Error;
                    }

                    var msgTelemetry = this.TelemetryFactory.BuildMessageTelemetry(string.Format(message, args), level);
                    this.TelemetryClient.Track(msgTelemetry);
                }
            }
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
    }
}
