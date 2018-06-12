using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
