namespace CCLLC.Telemetry
{
    /// <summary>
    /// Provides diagnostic logging within the Telemetry system.
    /// </summary>
    public interface IEventLogger
    {
        void FailedToSend(string msg, string appDomain = "Incorrect");

        void LogError(string msg, string appDomainName = "Incorrect");

        void LogVerbose(string msg, string appDomainName = "Incorrect");
    }
}
