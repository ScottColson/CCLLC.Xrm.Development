using System;
using System.Collections.Generic;

namespace CCLCC.Telemetry.DataContract
{
    [System.Diagnostics.Tracing.EventData]
    public class ExceptionDetails : IExceptionDetails
    {
        public int id { get; set; }
        public int outerId { get; set; }
        public string typeName { get; set; }
        public string message { get; set; }
        public bool hasFullStack { get; set; }
        public string stack { get; set; }
        public IList<IStackFrame> parsedStack { get; set; }

        internal static IExceptionDetails CreateWithoutStackInfo(Exception exception, IExceptionDetails parentExceptionDetails)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            var exceptionDetails = new ExceptionDetails()
            {
                id = exception.GetHashCode(),
                typeName = exception.GetType().FullName,
                message = exception.Message
            };

            if (parentExceptionDetails != null)
            {
                exceptionDetails.outerId = parentExceptionDetails.id;
            }

            return exceptionDetails;
        }

    }
}
