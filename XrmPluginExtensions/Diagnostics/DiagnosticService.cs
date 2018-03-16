using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace D365.XrmPluginExtensions.Diagnostics
{
    public class DiagnosticService : IDiagnosticService
    {
        internal DiagnosticService(ITracingService traceService)
        {




        }

        public string ProcessName => throw new NotImplementedException();

        public bool EnableDebugLogging { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void EnterMethod([CallerMemberName] string methodname = "")
        {
            throw new NotImplementedException();
        }

        public void ExitMethod(string message = null)
        {
            throw new NotImplementedException();
        }

       


        public void Trace(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Trace(string message, eSeverityLevel severity, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
