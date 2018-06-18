using System;
using CCLLC.Telemetry;

namespace CCLLC.Xrm.AppInsights
{
    public class XrmAppInsightsClient : IXrmAppInsightsClient
    {
        private IEventLogger logger;
        public ITelemetryClient TelemetryClient { get; private set; }
        public ITelemetryFactory TelemetryFactory { get; private set; }

        internal protected XrmAppInsightsClient(ITelemetryFactory factory, IComponentTelemetryClient client, IEventLogger logger)
        {
            this.logger = logger;
            this.TelemetryFactory = factory;
            this.TelemetryClient = client;
        }

        public IOperationalTelemetryClient<IDependencyTelemetry> StartDependencyOperation(string dependencyType = "Web", string target = "", string dependencyName ="PluginWebRequest")
        {
            var dependencyTelemetry = TelemetryFactory.BuildDependencyTelemetry(
                    dependencyType,
                    target,
                    dependencyName,
                    null);

            return TelemetryClient.StartOperation<IDependencyTelemetry>(dependencyTelemetry);
        }

        public void Trace(string message, params object[] args)
        {
            Trace(eSeverityLevel.Information, message, args);
        }

        public void Trace(eSeverityLevel level, string message, params object[] args)
        {
            try
            {
                if (this.TelemetryFactory != null && this.TelemetryClient != null && !string.IsNullOrEmpty(message))
                {
                    if (this.TelemetryClient != null && this.TelemetryFactory != null)
                    {
                        var msgTelemetry = this.TelemetryFactory.BuildMessageTelemetry(string.Format(message, args), level);
                        this.TelemetryClient.Track(msgTelemetry);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
            }

        }

        public void TrackEvent(string name)
        {
            try
            {
                if (this.TelemetryFactory != null && this.TelemetryClient != null && !string.IsNullOrEmpty(name))
                {
                    this.TelemetryClient.Track(this.TelemetryFactory.BuildEventTelemetry(name));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public void TrackException(Exception ex)
        {
            try
            {
                if (this.TelemetryFactory != null && this.TelemetryClient != null && ex != null)
                {
                    this.TelemetryClient.Track(this.TelemetryFactory.BuildExceptionTelemetry(ex));
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }
    }
}
