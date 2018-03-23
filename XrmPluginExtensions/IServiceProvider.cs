using System;
using CCLCC.XrmPluginExtensions.Telemetry;

namespace CCLCC.XrmPluginExtensions
{
    public interface IServiceProvider<T> : IServiceProvider where T : ITelemetryService
    {
        object GetService(Type serviceType);
    }
}