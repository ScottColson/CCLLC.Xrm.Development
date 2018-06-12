using System.IO;

namespace CCLLC.Telemetry
{
    public interface IJsonWriterFactory
    {
        IJsonWriter BuildJsonWriter(TextWriter textWriter);
    }
}
