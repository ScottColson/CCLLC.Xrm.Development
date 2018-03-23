using System;
using System.Runtime.CompilerServices;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmPluginExtensions.Diagnostics
{
    using Telemetry;

    public class DiagnosticService<T> : IDiagnosticService<T> where T : ITelemetryService, IDisposable
    {
        private ITracingService tracingService;
        private ITelemetryProvider<T> telemetryProvider;
        private IExecutionContext executionContext;
        private T telemetryService;
        private string pluginClassName;

        internal DiagnosticService(string pluginClassName, IExecutionContext executionContext, ITracingService tracingService, ITelemetryProvider<T> telemetryProvider)
        {
            this.tracingService = tracingService;
            this.telemetryProvider = telemetryProvider;
            this.executionContext = executionContext;
            this.pluginClassName = pluginClassName;
        }     
       
        public T Telemetry
        {
            get
            {
                if(telemetryService == null)
                {
                    telemetryService = telemetryProvider.CreateTelemetryService(pluginClassName, telemetryProvider, tracingService, executionContext);
                }
                return telemetryService;
            }
        }

        public void Dispose()
        {            
            if(telemetryService != null)
            {
                telemetryService.Dispose();
            }
        }

        public void EnterMethod([CallerMemberName] string methodname = "")
        {
            Trace("Entered: {0}", methodname);
        }

        public void ExitMethod(string message = null)
        {
            Trace("Exiting method");
        }

        public void Trace(string format, params object[] args)
        {
            if (Telemetry.IsInitialized)
            {
                Telemetry.TrackTrace(eSeverityLevel.Information, format, args);
            }
           
            if(!Telemetry.IsInitialized || !Telemetry.WritesToPluginTracLog)
            {
                tracingService.Trace(format, args);
            }
        }

        public void TraceGeneralException(Exception ex)
        {
            if (Telemetry.IsInitialized)
            {
                Telemetry.TrackException(ex);
            }

            if (!Telemetry.IsInitialized || !Telemetry.WritesToPluginTracLog)
            {
                tracingService.Trace("Unhandled Exception: {0}", ex.Message);
            }
        }

        public void TracePluginException(InvalidPluginExecutionException ex)
        {
            if (Telemetry.IsInitialized)
            {
                Telemetry.TrackTrace(eSeverityLevel.Error, "Plugin Exception: {0}", ex.Message);
            }

            if (!Telemetry.IsInitialized || !Telemetry.WritesToPluginTracLog)
            {
                tracingService.Trace("Plugin Exception: {0}", ex.Message);
            }
        }
    }
}
