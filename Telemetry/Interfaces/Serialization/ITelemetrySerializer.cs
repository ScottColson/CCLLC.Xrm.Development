using System.Collections.Generic;
using System.Text;

namespace CCLLC.Telemetry
{
    public interface ITelemetrySerializer
    {
        UTF8Encoding TransmissionEncoding { get; }

        string CompressionType { get; }
        string ContentType { get; }
        byte[] Serialize(IEnumerable<ITelemetry> telemetryItems, bool compress = true);

        void SerializeExceptions(IEnumerable<IExceptionDetails> exceptions, IJsonWriter writer);
    }       
}
