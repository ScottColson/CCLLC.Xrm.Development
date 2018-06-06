using CCLLC.Telemetry;

namespace TelemetrySamples.Telemetry
{
    /// <summary>
    /// Adds a custom property wiht the name and value specified to the telemetry context 
    /// of each telemetry item processed by the telemetry sink.
    /// </summary>
    public class CustomPluginPropertyProcessor : CCLLC.Telemetry.ITelemetryProcessor
    {
        private string _propertyName;
        private string _propertyValue;

        public CustomPluginPropertyProcessor(string propertyName, string propertyValue)
        {
            _propertyName = propertyName;
            _propertyValue = propertyValue;
        }

        public void Process(ITelemetry telemetryItem)
        {
            if(telemetryItem != null && telemetryItem.Context != null && !string.IsNullOrEmpty(_propertyName) && !string.IsNullOrEmpty(_propertyValue))
            {
                telemetryItem.Context.Properties.Add(_propertyName, _propertyValue);
            }
        }
    }
}
