namespace CCLLC.Telemetry
{
    /// <summary>
    /// Provides diagnostic logging within the Telemetry system.
    /// </summary>
    public interface IEventLogger
    {
        void FailedToSend(string msg, string appDomain = "Incorrect");
    }
}
