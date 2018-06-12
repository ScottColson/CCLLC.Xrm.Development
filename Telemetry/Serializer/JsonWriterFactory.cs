using System.IO;

namespace CCLLC.Telemetry.Serializer
{
    public class JsonWriterFactory : IJsonWriterFactory
    {
        public IJsonWriter BuildJsonWriter(TextWriter textWriter)
        {
            return new JsonWriter(textWriter);
        }
    }
}
