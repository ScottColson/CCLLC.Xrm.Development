using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using CCLLC.Telemetry.Implementation;

namespace CCLLC.Telemetry.Serializer
{
    public class TelemetrySerializer : ITelemetrySerializer
    {
        private IContextTagKeys _contextTagKeys;
        private IJsonWriterFactory _jsonWriterFactory;
        private readonly UTF8Encoding _transmissionEncoding = new UTF8Encoding(false);

        public virtual UTF8Encoding TransmissionEncoding { get { return _transmissionEncoding; } }

        /// <summary>
        /// Gets the compression type used by the serializer. 
        /// </summary>
        public virtual string CompressionType
        {
            get
            {
                return "gzip";
            }
        }

        /// <summary>
        /// Gets the content type used by the serializer. 
        /// </summary>
        public virtual string ContentType
        {
            get
            {
                return "application/x-json-stream";
            }
        }

        public TelemetrySerializer(IJsonWriterFactory jsonWriterFactory, IContextTagKeys contextTagKeys)
        {
            if (jsonWriterFactory == null) { throw new ArgumentNullException("jsonWriterFactory"); }
            if (contextTagKeys == null) { throw new ArgumentNullException("contextTagKeys"); }
            _jsonWriterFactory = jsonWriterFactory;
            _contextTagKeys = contextTagKeys;

        }

        /// <summary>
        /// Serializes and compress the telemetry items into an array of JSON objects. 
        /// </summary>
        /// <param name="telemetryItems">The list of telemetry items to serialize.</param>
        /// <param name="compress">Should serialization also perform compression.</param>
        /// <returns>The compressed and serialized telemetry items.</returns>       
        public virtual byte[] Serialize(IEnumerable<ITelemetry> telemetryItems, bool compress = true)
        {
            var memoryStream = new MemoryStream();
            using (Stream stream = compress ? CreateCompressedStream(memoryStream) : memoryStream)
            {
                using (var streamWriter = new StreamWriter(stream, TransmissionEncoding))
                {
                    SerializeToStream(telemetryItems, streamWriter);
                }
            }

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Serializes <paramref name="telemetryItems"/> into an array and write the response to <paramref name="streamWriter"/>.
        /// </summary>
        protected virtual void SerializeToStream(IEnumerable<ITelemetry> telemetryItems, TextWriter streamWriter)
        {
            var jsonWriter = _jsonWriterFactory.BuildJsonWriter(streamWriter);

            int telemetryCount = 0;
            jsonWriter.WriteStartArray();

            foreach (var telemetryItem in telemetryItems)
            {
                if (telemetryCount++ > 0)
                {
                    streamWriter.Write(",");                    
                }

                telemetryItem.Sanitize();
                Serialize(telemetryItem, jsonWriter);
            }

            jsonWriter.WriteEndArray();
        }

        /// <summary>
        /// Serialize the <see cref="ITelemetry"/> item into a JSON object. All context and data items are fields
        /// in the object. If the item has custom properties or measurements then those are serialized as a list 
        /// name/value pairs. Exception details, if present are serialized as an array of exception information which
        /// is a complex object.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="writer"></param>
        protected virtual void Serialize(ITelemetry item, IJsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WriteProperty("telemetrytype", item.TelemetryName);
            writer.WriteProperty("time", item.Timestamp.UtcDateTime.ToString("o", CultureInfo.InvariantCulture));

            var samplingSupportingTelemetry = item as ISupportSampling;
            if (samplingSupportingTelemetry != null
                && samplingSupportingTelemetry.SamplingPercentage.HasValue
                && (samplingSupportingTelemetry.SamplingPercentage.Value > 0.0 + 1.0E-12)
                && (samplingSupportingTelemetry.SamplingPercentage.Value < 100.0 - 1.0E-12))
            {
                writer.WriteProperty("sampleRate", samplingSupportingTelemetry.SamplingPercentage.Value);
            }

            writer.WriteProperty("seq", item.Sequence);

            //serialize out the telemetry context into the current object
            if (item.Context != null)
            {
                var contextTags = item.Context.ToContextTags(_contextTagKeys);
                foreach (var tag in contextTags)
                {
                    writer.WriteProperty(tag.Key, tag.Value);
                }
            }

            //serialize the main data tags for the telemetry.
            var dataTags = item.GetTaggedData();
            foreach (var tag in dataTags)
            {
                writer.WriteProperty(tag.Key, tag.Value);
            }

            //if the telemetry has additional properties then serialize the properties into a
            //list of name/value pairs.
            var withProperties = item as ISupportProperties;
            if (withProperties != null && withProperties.Properties.Count > 0)
            {
                writer.WriteProperty("properties", withProperties.Properties);
            }

            //if the telemetry has measurements then serialize the measurements into a
            //list of name/value pairs.
            var withMeasurements = item as ISupportMetrics;
            if (withMeasurements != null && withMeasurements.Metrics.Count > 0)
            {
                writer.WriteProperty("measurements", withMeasurements.Metrics);
            }

            //if the telemtry has a list of exception details then serialize the list
            //into a JSON array.
            var withExceptions = item as IExceptionTelemetry;
            if (withExceptions != null && withExceptions.ExceptionDetails.Count > 0)
            {
                writer.WritePropertyName("exceptions");
                {
                    writer.WriteStartArray();

                    SerializeExceptions(withExceptions.ExceptionDetails, writer);

                    writer.WriteEndArray();
                }
            }

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Creates a GZIP compression stream that wraps <paramref name="stream"/>. For windows phone 8.0 it returns <paramref name="stream"/>. 
        /// </summary>
        protected virtual Stream CreateCompressedStream(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Compress);
        }
        
        /// <summary>
        /// Serialize the list of exception details into an object that represents the exception and
        /// related stack.
        /// </summary>
        /// <param name="exceptions"></param>
        /// <param name="writer"></param>
        public virtual void SerializeExceptions(IEnumerable<IExceptionDetails> exceptions, IJsonWriter writer)
        {
            int exceptionArrayIndex = 0;

            foreach (IExceptionDetails exceptionDetails in exceptions)
            {
                if (exceptionArrayIndex++ != 0)
                {
                    writer.WriteComma();
                }

                writer.WriteStartObject();
                writer.WriteProperty("id", exceptionDetails.id);
                if (exceptionDetails.outerId != 0)
                {
                    writer.WriteProperty("outerId", exceptionDetails.outerId);
                }

                writer.WriteProperty(
                    "typeName",
                    Utils.PopulateRequiredStringValue(exceptionDetails.typeName));
                writer.WriteProperty(
                    "message",
                    Utils.PopulateRequiredStringValue(exceptionDetails.message));

                if (exceptionDetails.hasFullStack)
                {
                    writer.WriteProperty("hasFullStack", exceptionDetails.hasFullStack);
                }

                writer.WriteProperty("stack", exceptionDetails.stack);

                if (exceptionDetails.parsedStack.Count > 0)
                {
                    writer.WritePropertyName("parsedStack");

                    writer.WriteStartArray();

                    int stackFrameArrayIndex = 0;

                    foreach (IStackFrame frame in exceptionDetails.parsedStack)
                    {
                        if (stackFrameArrayIndex++ != 0)
                        {
                            writer.WriteComma();
                        }

                        writer.WriteStartObject();
                        SerializeStackFrame(frame, writer);
                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                }

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize the stack frame for exception detials.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="writer"></param>
        protected virtual void SerializeStackFrame(IStackFrame frame, IJsonWriter writer)
        {
            writer.WriteProperty("level", frame.level);
            writer.WriteProperty(
                "method",
                Utils.PopulateRequiredStringValue(frame.method));
            writer.WriteProperty("assembly", frame.assembly);
            writer.WriteProperty("fileName", frame.fileName);

            // 0 means it is unavailable
            if (frame.line != 0)
            {
                writer.WriteProperty("line", frame.line);
            }
        }
    }

}
