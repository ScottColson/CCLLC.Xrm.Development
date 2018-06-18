namespace CCLLC.Telemetry.EventLogger
{
    public class InertEventLogger : IEventLogger
    {
        public void FailedToSend(string msg, string appDomain = "Incorrect")
        {            
        }

        public void LogError(string msg, string appDomainName = "Incorrect")
        {            
        }

        public void LogVerbose(string msg, string appDomainName = "Incorrect")
        {             
        }
    }
}
