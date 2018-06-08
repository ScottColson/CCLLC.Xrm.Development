using CCLLC.Telemetry;

namespace TelemetrySamples.Telemetry
{
    /// <summary>
    /// Adds a custom property with the name and value specified to the telemetry context 
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
                //need to cast the telemetry item to an interface that supports properties.
                var withProperties = telemetryItem as ISupportProperties;
                if (withProperties != null)
                {
                    withProperties.Properties.Add(_propertyName, _propertyValue);
                }
            }
        }
    }
}
