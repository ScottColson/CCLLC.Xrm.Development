using System;
using Microsoft.Xrm.Sdk;
using CCLCC.Telemetry;
using CCLCC.Xrm;
using CCLCC.Xrm.Context;
using CCLCC.Telemetry.Sink;

namespace XrmPluginTestHarness
{
    public class SamplePlugin : PluginBase<Entity>, IPlugin
    {
        public SamplePlugin(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
            base.RegisterMessageHandler(null, null, ePluginStage.PostOperation, ExecuteHandler);
        }

        public void ExecuteHandler(ILocalContext<Entity> localContext)
        {
            
            localContext.TelemetryClient.TelemetrySink.OnConfigure = () =>
            {
                var sink = localContext.TelemetryClient.TelemetrySink;
                sink.Channel.EndpointAddress = new Uri("https://dc.services.visualstudio.com/v2/track"); //Application Insights
                sink.ProcessChain.TelemetryProcessors.Add(new SequencePropertyProcessor());
                sink.ProcessChain.TelemetryProcessors.Add(new InstrumentationKeyPropertyProcessor("7a6ecb67-6c9c-4640-81d2-80ce76c3ca34"));

                return true; //indicate that the delegate successfully configured the sink.
            };

            localContext.Trace("Simple trace message.");
            //localContext.Trace(SeverityLevel.Error, "Error message");
            //localContext.Trace("Parameterized message at {0}", DateTime.Now);

            localContext.TelemetryClient.TelemetrySink.Channel.Flush();
        }
    }
}
