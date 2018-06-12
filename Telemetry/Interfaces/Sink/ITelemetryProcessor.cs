namespace CCLLC.Telemetry
{
    public interface ITelemetryProcessor
    {
        void Process(ITelemetry telemetryItem);
    }
}
