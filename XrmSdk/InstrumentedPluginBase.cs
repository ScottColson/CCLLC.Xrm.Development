using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CCLLC.Core;
using CCLLC.Telemetry;
using CCLLC.Telemetry.EventLogger;
using CCLLC.Telemetry.Client;
using CCLLC.Telemetry.Context;
using CCLLC.Telemetry.Serializer;
using CCLLC.Telemetry.Sink;
using Microsoft.Xrm.Sdk;

namespace CCLLC.Xrm.Sdk
{
    public abstract class InstrumentedPluginBase : PluginBase, ISupportPluginInstrumentation
    {
        private static IIocContainer _container;
        private static object _containerLock = new object();
        private static ITelemetrySink _telemetrySink;
        private static object _sinkLock = new object();

        /// <summary>
        /// Provides an <see cref="IIocContainer"/> instance to register all objects used by the
        /// base plugin. This container uses a static implementation therefore all 
        /// plugins that use this base share the same container and therefore
        /// use the same concreate implementations registered in the container.
        /// </summary>
        /// <remarks>
        /// Overrides BasePlugin implementation because telementry requires additional
        /// dependencies that may not be in the BasePlugin container static member.
        /// </remarks>
        public override IIocContainer Container
        {
            get
            {
                if (_container == null)
                {
                    lock (_containerLock)
                    {
                        if (_container == null)
                        {
                            _container = new IocContainer();
                            RegisterContainerServices();
                        }
                    }

                }
                return _container;
            }
        }

        /// <summary>
        /// Provides a <see cref="ITelemetrySink"/> to recieve and process various 
        /// <see cref="ITelemetry"/> items generated during the execution of the 
        /// Plugin. This sink uses a static implementation therefore all 
        /// plugins that use this base share the same sink which is more
        /// efficient than operating multiple sinks.
        /// </summary>
        public virtual ITelemetrySink TelemetrySink
        {
            get
            {
                if (_telemetrySink == null)
                {
                    lock (_sinkLock)
                    {
                        if (_telemetrySink == null)
                        {
                            _telemetrySink = Container.Resolve<ITelemetrySink>();                            
                        }
                    }
                }

                return _telemetrySink;
            }
        }

        public InstrumentedPluginBase(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
        }

        public override void RegisterContainerServices()
        {
            base.RegisterContainerServices();

            //Telemetry issue event logger
            Container.RegisterAsSingleInstance<IEventLogger, InertEventLogger>();

            //setup the objects needed to create/capture telemetry items.
            Container.RegisterAsSingleInstance<ITelemetryFactory, TelemetryFactory>();  //ITelemetryFactory is used to create new telemetry items.
            Container.RegisterAsSingleInstance<ITelemetryClientFactory, TelemetryClientFactory>(); //ITelemetryClientFactory is used to create and configure a telemetry client.
            Container.RegisterAsSingleInstance<IXrmTelemetryPropertyManager, Telemetry.ExecutionContextPropertyManager>(); //Plugin property manager.
            Container.Register<ITelemetryContext, TelemetryContext>(); //ITelemetryContext is a dependency for telemetry creation.
            Container.Register<ITelemetryInitializerChain, TelemetryInitializerChain>(); //ITelemetryInitializerChain is a dependency for building a telemetry client.

            //setup the objects needed to buffer and send telemetry to Application Insights.
            Container.Register<ITelemetrySink, TelemetrySink>(); //ITelemetrySink receives telemetry from one or more telemetry clients, processes it, buffers it, and transmits it.
            Container.Register<ITelemetryProcessChain, TelemetryProcessChain>(); //ITelemetryProcessChain holds 0 or more processors that can modify the telemetry prior to transmission.
            Container.Register<ITelemetryChannel, SyncMemoryChannel>(); //ITelemetryChannel provides the buffering and transmission. There is a sync and an asynch channel.
            Container.Register<ITelemetryBuffer, TelemetryBuffer>(); //ITelemetryBuffer is used the channel
            Container.Register<ITelemetryTransmitter, AITelemetryTransmitter>(); //ITelemetryTransmitter transmits a block of telemetry to Applicatoin Insights.

            //setup the objects needed to serialize telemetry as part of transmission.
            Container.Register<IContextTagKeys, AIContextTagKeys>(); //Defines context tags expected by Application Insights.
            Container.Register<ITelemetrySerializer, AITelemetrySerializer>(); //Serialize telemetry items into a compressed Gzip data.
            Container.Register<IJsonWriterFactory, JsonWriterFactory>(); //Factory to create JSON converters as needed.
        }

        /// <summary>
        /// Flag to capture execution execution performance metrics using request telemetry.
        /// </summary>
        public bool TrackExecutionPerformance { get; set; }

        /// <summary>
        /// Flag to force flushing the telementry sink buffer after handler execution completes.
        /// </summary>
        public bool FlushTelemetryAfterExecution { get; set; }

        public virtual bool ConfigureTelemetrySink(ILocalPluginContext localContext)
        {
            if (localContext != null)
            {
                var key = localContext.ExtensionSettings.Get<string>("Telemetry.InstrumentationKey");
               
                if (!string.IsNullOrEmpty(key))
                {
                    TelemetrySink.ProcessChain.TelemetryProcessors.Add(new SequencePropertyProcessor());
                    TelemetrySink.ProcessChain.TelemetryProcessors.Add(new InstrumentationKeyPropertyProcessor(key));
                    
                    return true; //telemetry sink is configured.
                }
            }

            return false; //telmetry sink is not configured.
        }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// Microsoft CRM plugins must be thread-safe and stateless. 
        /// </remarks>
        public override void Execute(IServiceProvider serviceProvider)
        {           
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var success = true;            

            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entering {0}.Execute()", this.GetType().ToString()));

            var executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            var telemetryFactory = Container.Resolve<ITelemetryFactory>();
            var telemetryClientFactory = Container.Resolve<ITelemetryClientFactory>();

            //Create a dictionary of custom properties based on values in the execution context.
            var propertyManager = Container.Resolve<IXrmTelemetryPropertyManager>();
            var properties = propertyManager.CreateXrmPropertiesDictionary(this.GetType().ToString(), executionContext);
            
            using (var telemetryClient = telemetryClientFactory.BuildClient(
                this.GetType().ToString(),
                this.TelemetrySink,
                properties))
            {

                #region Setup Telementry Context

                //capture execution context attributes that fit in telemetry context
                telemetryClient.Context.Operation.Name = executionContext.MessageName;
                telemetryClient.Context.Operation.CorrelationVector = executionContext.CorrelationId.ToString();
                telemetryClient.Context.Operation.Id = executionContext.OperationId.ToString();
                telemetryClient.Context.Session.Id = executionContext.CorrelationId.ToString();

                //Capture data contexty if the telemetry provider supports data key context.
                var asDataContext = telemetryClient.Context as ISupportDataKeyContext;
                if (asDataContext != null)
                {
                    asDataContext.Data.RecordSource = executionContext.OrganizationName;
                    asDataContext.Data.RecordType = executionContext.PrimaryEntityName;
                    asDataContext.Data.RecordId = executionContext.PrimaryEntityId.ToString();
                }

                #endregion
                               
                try
                {
                    var matchingHandlers = this.PluginEventHandlers
                        .Where(a => (int)a.Stage == executionContext.Stage
                            && (string.IsNullOrWhiteSpace(a.MessageName) || string.Compare(a.MessageName, executionContext.MessageName, StringComparison.InvariantCultureIgnoreCase) == 0)
                            && (string.IsNullOrWhiteSpace(a.EntityName) || string.Compare(a.EntityName, executionContext.PrimaryEntityName, StringComparison.InvariantCultureIgnoreCase) == 0));

                    if (matchingHandlers.Any())
                    {
                        var localContextFactory = Container.Resolve<ILocalPluginContextFactory>();

                        using (var localContext = localContextFactory.BuildLocalPluginContext(executionContext, serviceProvider, this.Container, telemetryClient))
                        {
                            if (!TelemetrySink.IsConfigured)
                            {
                                TelemetrySink.OnConfigure = () => { return this.ConfigureTelemetrySink(localContext); };
                            }

                            foreach (var handler in matchingHandlers)
                            {
                                try
                                {
                                    handler.PluginAction.Invoke(localContext);
                                }
                                catch (InvalidPluginExecutionException ex)
                                {
                                    success = false;
                                    if (telemetryClient != null && telemetryFactory != null)
                                    {
                                        telemetryClient.Track(telemetryFactory.BuildMessageTelemetry(ex.Message, eSeverityLevel.Error));
                                    }
                                    throw;
                                }
                                catch (Exception ex)
                                {
                                    success = false;
                                    if (telemetryClient != null && telemetryFactory != null)
                                    {
                                        telemetryClient.Track(telemetryFactory.BuildExceptionTelemetry(ex));
                                    }
                                    throw;
                                }
                                finally
                                {
                                    if (this.TrackExecutionPerformance && telemetryFactory != null && telemetryClient != null)
                                    {
                                        var r = telemetryFactory.BuildRequestTelemetry("PluginExecution", null, new Dictionary<string, string> { { "handlerName", handler.Id } });
                                        r.Duration = sw.Elapsed;
                                        r.ResponseCode = "200";
                                        r.Success = success;

                                        telemetryClient.Track(r);
                                    }

                                    if (this.FlushTelemetryAfterExecution && telemetryClient != null)
                                    {
                                        telemetryClient.Flush();
                                    }

                                    sw.Restart();
                                    
                                }
                            }
                        } //using localContext
                    }
                }
                catch (InvalidPluginExecutionException ex)
                {                    
                    tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                    throw;
                }
                catch (Exception ex)
                {                    
                    tracingService.Trace(string.Format("Exception: {0}", ex.Message));
                    throw new InvalidPluginExecutionException(string.Format("Unhandled Plugin Exception {0}", ex.Message), ex);
                }
                
                
            } //using telemetryClient.

            sw.Stop();
            sw = null;

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.GetType().ToString()));
        }
    }
}
