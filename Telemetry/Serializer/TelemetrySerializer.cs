using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Serializer
{
    using Interfaces;
    using Implementation;

    using Telemetry;

    public class TelemetrySerializer : ITelemetrySerializer
    {
        private IContextTagKeys contextTagKeys;
        private readonly UTF8Encoding transmissionEncoding = new UTF8Encoding(false);
        public virtual UTF8Encoding TransmissionEncoding { get { return this.transmissionEncoding; } }

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

        public TelemetrySerializer(IContextTagKeys contextTagKeys)
        {
            if(contextTagKeys == null) { throw new ArgumentNullException("contextTagKeys"); }
            this.contextTagKeys = contextTagKeys;
        }

        /// <summary>
        /// Serializes and compress the telemetry items into a JSON string. Each JSON object is separated by a new line. 
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
                    SeializeToStream(telemetryItems, streamWriter);
                }
            }

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Serializes <paramref name="telemetryItems"/> and write the response to <paramref name="streamWriter"/>.
        /// </summary>
        protected virtual void SeializeToStream(IEnumerable<ITelemetry> telemetryItems, TextWriter streamWriter)
        {
            JsonWriter jsonWriter = new JsonWriter(streamWriter);

            int telemetryCount = 0;
            foreach (var telemetryItem in telemetryItems)
            {
                if (telemetryCount++ > 0)
                {
                    streamWriter.Write(Environment.NewLine);
                }

                telemetryItem.Sanitize();
                Serialize(telemetryItem, jsonWriter);
            }
        }


        protected virtual void Serialize(ITelemetry item, IJsonWriter writer)
        {
            writer.WriteStartObject();

            WriteTelemetryName(item, writer);
            WriteEnvelopeProperties(item, writer, contextTagKeys);
            writer.WritePropertyName("data");
            {
                writer.WriteStartObject();

                var dataModelItem = item as IDataModelTelemetry<IDataModel>;
                if (dataModelItem != null)
                {
                    writer.WriteProperty("baseType", dataModelItem.BaseType);
                    writer.WritePropertyName("baseData");
                    {
                        writer.WriteStartObject();

                        dataModelItem.SerializeData(this, writer);

                        writer.WriteEndObject();
                    }
                }
                writer.WriteEndObject();
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

        protected virtual void WriteTelemetryName(ITelemetry telemetry, IJsonWriter json)
        {
            // A different event name prefix is sent for normal mode and developer mode.
            bool isDevMode = false;
            string devModeProperty;
            var telemetryWithProperties = telemetry as ISupportProperties;
            if (telemetryWithProperties != null && telemetryWithProperties.Properties.TryGetValue("DeveloperMode", out devModeProperty))
            {
                bool.TryParse(devModeProperty, out isDevMode);
            }

            // Format the event name using the following format:
            // Microsoft.ApplicationInsights[.Dev].<normalized-instrumentation-key>.<event-type>
            var eventName = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0}{1}{2}",
                isDevMode ? AIConstants.DevModeTelemetryNamePrefix : AIConstants.TelemetryNamePrefix,
                NormalizeInstrumentationKey(telemetry.Context.InstrumentationKey),
                telemetry.TelemetryName);
            json.WriteProperty("name", eventName);
        }

        protected virtual void WriteEnvelopeProperties(ITelemetry telemetry, IJsonWriter json, IContextTagKeys keys)
        {
            json.WriteProperty("time", telemetry.Timestamp.UtcDateTime.ToString("o", CultureInfo.InvariantCulture));

            var samplingSupportingTelemetry = telemetry as ISupportSampling;

            if (samplingSupportingTelemetry != null
                && samplingSupportingTelemetry.SamplingPercentage.HasValue
                && (samplingSupportingTelemetry.SamplingPercentage.Value > 0.0 + 1.0E-12)
                && (samplingSupportingTelemetry.SamplingPercentage.Value < 100.0 - 1.0E-12))
            {
                json.WriteProperty("sampleRate", samplingSupportingTelemetry.SamplingPercentage.Value);
            }

            json.WriteProperty("seq", telemetry.Sequence);
            WriteTelemetryContext(json, telemetry.Context, keys);
        }

        protected virtual void WriteTelemetryContext(IJsonWriter json, ITelemetryContext context, IContextTagKeys keys)
        {
            if (context != null)
            {
                json.WriteProperty("iKey", context.InstrumentationKey);
                json.WriteProperty("tags", context.ToContextTags(keys));
            }
        }

        /// <summary>
        /// Normalize instrumentation key by removing dashes ('-') and making string in the lowercase.
        /// In case no InstrumentationKey is available just return empty string.
        /// In case when InstrumentationKey is available return normalized key + dot ('.')
        /// as a separator between instrumentation key part and telemetry name part.
        /// </summary>
        protected virtual string NormalizeInstrumentationKey(string instrumentationKey)
        {
            if (instrumentationKey.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            return instrumentationKey.Replace("-", string.Empty).ToLowerInvariant() + ".";
        }

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
