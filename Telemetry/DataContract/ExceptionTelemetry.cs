using System;
using System.Collections.Generic;
using System.Globalization;
using CCLLC.Telemetry.Implementation;

namespace CCLLC.Telemetry.DataContract
{
    public class ExceptionTelemetry : TelemetryBase<IExceptionDataModel>, IExceptionTelemetry, ISupportProperties, ISupportMetrics, ISupportSampling
    {
        private Exception exception;
        public Exception Exception
        {
            get
            {
                return this.exception;
            }

            set
            {
                this.exception = value;
                this.UpdateExceptions(value);
            }
        }

        private string message;
        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                this.message = value;

                if (this.Data.exceptions != null && this.Data.exceptions.Count > 0)
                {
                    this.Data.exceptions[0].message = value;
                }
                else
                {
                    this.UpdateExceptions(this.Exception);
                }
            }
        }

        public IList<IExceptionDetails> ExceptionDetails
        {
            get { return this.Data.exceptions; }
        }

        public IDictionary<string, double> Metrics
        {
            get { return this.Data.measurements; }
        }

        public IDictionary<string, string> Properties
        {
            get { return this.Data.properties; }
        }

        public double? SamplingPercentage { get; set; }        
       
        public ExceptionTelemetry(Exception ex, ITelemetryContext context, IExceptionDataModel data, IDictionary<string, string> telemetryProperties = null, IDictionary<string,double> telemetryMetrics = null) : base("Exception", context, data)
        {           
            this.Exception = ex;

            if (telemetryProperties != null && telemetryProperties.Count > 0)
            {
                Utils.CopyDictionary<string>(telemetryProperties, this.Properties);
            }

            if (telemetryMetrics != null && telemetryMetrics.Count > 0)
            {
                Utils.CopyDictionary<double>(telemetryMetrics, this.Metrics);
            }
        }

        internal ExceptionTelemetry(IExceptionTelemetry source):this(source.Exception, source.Context.DeepClone(), source.Data.DeepClone<IExceptionDataModel>())
        {
            this.Sequence = source.Sequence;
            this.Timestamp = source.Timestamp;
            this.SamplingPercentage = source.SamplingPercentage;
            this.Exception = source.Exception;
        }

        public override IDataModelTelemetry<IExceptionDataModel> DeepClone()
        {
            return new ExceptionTelemetry(this);
        }

        public override void Sanitize()
        {
            // Sanitize on the ExceptionDetails stack information for raw stack and parsed stack is done while creating the object in ExceptionConverter.cs
            this.Properties.SanitizeProperties();
            this.Metrics.SanitizeMeasurements();
        }

        public override void SerializeData(ITelemetrySerializer serializer, IJsonWriter writer)
        {
            writer.WriteProperty("ver", this.Data.ver);
            writer.WriteProperty("problemId", this.Data.problemId);
            writer.WriteProperty("properties", this.Data.properties);
            writer.WriteProperty("measurements", this.Data.measurements);
            writer.WritePropertyName("exceptions");
            {
                writer.WriteStartArray();

                serializer.SerializeExceptions(this.Data.exceptions, writer);

                writer.WriteEndArray();
            }

            if (this.Data.severityLevel.HasValue)
            {
                writer.WriteProperty("severityLevel", this.Data.severityLevel.Value.ToString());
            }
        }

        public override IDictionary<string, string> GetTaggedData()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("ver", this.Data.ver.ToString());
            dict.Add("problemId", this.Data.problemId);
            if (this.Data.severityLevel.HasValue)
            {
                dict.Add("severityLevel", this.Data.severityLevel.Value.ToString());
            }

            return dict;
        }


        private void ConvertExceptionTree(Exception exception, IExceptionDetails parentExceptionDetails, List<IExceptionDetails> exceptions)
        {
            if (exception == null)
            {
                exception = new Exception(Utils.PopulateRequiredStringValue("message"));
            }

            var exceptionDetails = ExceptionConverter.ConvertToExceptionDetails(exception, parentExceptionDetails);

            // For upper level exception see if Message was provided and do not use exceptiom.message in that case
            if (parentExceptionDetails == null && !string.IsNullOrWhiteSpace(this.Message))
            {
                exceptionDetails.message = this.Message;
            }

            exceptions.Add(exceptionDetails);

            AggregateException aggregate = exception as AggregateException;
            if (aggregate != null)
            {
                foreach (Exception inner in aggregate.InnerExceptions)
                {
                    this.ConvertExceptionTree(inner, exceptionDetails, exceptions);
                }
            }
            else if (exception.InnerException != null)
            {
                this.ConvertExceptionTree(exception.InnerException, exceptionDetails, exceptions);
            }
        }

        private void UpdateExceptions(Exception exception)
        {
            // collect the set of exceptions detail info from the passed in exception
            List<IExceptionDetails> exceptions = new List<IExceptionDetails>();
            this.ConvertExceptionTree(exception, null, exceptions);

            // trim if we have too many, also add a custom exception to let the user know we're trimmed
            if (exceptions.Count > AIConstants.MaxExceptionCountToSave)
            {                
                InnerExceptionCountExceededException countExceededException = new InnerExceptionCountExceededException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The number of inner exceptions was {0} which is larger than {1}, the maximum number allowed during transmission. All but the first {1} have been dropped.",
                        exceptions.Count,
                        AIConstants.MaxExceptionCountToSave));

                // remove all but the first N exceptions
                exceptions.RemoveRange(AIConstants.MaxExceptionCountToSave, exceptions.Count - AIConstants.MaxExceptionCountToSave);

                // we'll add our new exception and parent it to the root exception (first one in the list)
                //exceptions.Add(ExceptionConverter.ConvertToExceptionDetails(countExceededException, exceptions[0]));
            }

            this.Data.exceptions = exceptions;
        }
    }
}
