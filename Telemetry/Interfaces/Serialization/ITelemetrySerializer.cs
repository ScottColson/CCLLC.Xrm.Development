using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
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
