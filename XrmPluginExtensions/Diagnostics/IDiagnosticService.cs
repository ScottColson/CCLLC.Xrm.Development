using System;
using System.Runtime.CompilerServices;

namespace D365.XrmPluginExtensions.Diagnostics
{
    public interface IDiagnosticService : IDisposable
    {
        string ProcessName { get; }
        bool EnableDebugLogging { get; set; }
        void EnterMethod([CallerMemberName]string methodname = "");
        void ExitMethod(string message = null);
        //void Save(ITraceLogWriter writer, bool saveInfoMessages);
        //void Save(ITraceLogWriter writer);
        
        void Trace(string message, eSeverityLevel severity, params object[] args);
        void Trace(string message, params object[] args);
    }
}
