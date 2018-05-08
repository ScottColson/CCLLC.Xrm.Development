namespace CCLCC.Telemetry.Interfaces
{
    public interface ITelemetryInitializer
    {
        /// <summary>
        /// Initializes properties of the specified <see cref="ITelemetry"/> object.
        /// </summary>
        void Initialize(ITelemetry telemetry);

    }
}
