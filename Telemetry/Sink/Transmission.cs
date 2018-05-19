using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CCLCC.Telemetry.Implementation;

namespace CCLCC.Telemetry.Sink
{
    public class Transmission
    {
        internal const string ContentTypeHeader = "Content-Type";
        internal const string ContentEncodingHeader = "Content-Encoding";
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(100);
        private int isSending;

        /// <summary>
        /// Gets the Address of the endpoint to which transmission will be sent.
        /// </summary>
        public Uri EndpointAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the content of the transmission.
        /// </summary>
        public byte[] Content
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the content's type of the transmission.
        /// </summary>
        public string ContentType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the encoding method of the transmission.
        /// </summary>
        public string ContentEncoding
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a timeout value for the transmission.
        /// </summary>
        public TimeSpan Timeout
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets an id of the transmission.
        /// </summary>
        public string Id
        {
            get; private set;
        }

        /// <summary>
        /// Gets the number of telemetry items in the transmission.
        /// </summary>
        public ICollection<ITelemetry> TelemetryItems
        {
            get; private set;
        }

        public Transmission(Uri address, byte[] content, string contentType, string contentEncoding, TimeSpan timeout = default(TimeSpan))
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            if (contentType == null)
            {
                throw new ArgumentNullException("contentType");
            }

            this.EndpointAddress = address;
            this.Content = content;
            this.ContentType = contentType;
            this.ContentEncoding = contentEncoding;
            this.Timeout = timeout == default(TimeSpan) ? DefaultTimeout : timeout;
            this.Id = Convert.ToBase64String(BitConverter.GetBytes(WeakConcurrentRandom.Instance.Next()));
            this.TelemetryItems = null;

        }


        /// <summary>
        /// Executes the request that the current transmission represents.
        /// </summary>
        /// <returns>The task to await.</returns>
        public virtual async Task<HttpWebResponseWrapper> SendAsync()
        {
            if (Interlocked.CompareExchange(ref this.isSending, 1, 0) != 0)
            {
                throw new InvalidOperationException("Transmission is already in progress.");
            }

            try
            {

                WebRequest request = this.CreateRequest(this.EndpointAddress);
                Task<HttpWebResponseWrapper> sendTask = this.GetResponseAsync(request);

                Task timeoutTask = Task.Delay(this.Timeout).ContinueWith(task =>
                {
                    if (!sendTask.IsCompleted)
                    {
                        request.Abort(); // And force the sendTask to throw WebException.
                    }
                });

                Task completedTask = await Task.WhenAny(timeoutTask, sendTask).ConfigureAwait(false);

                // Observe any exceptions the sendTask may have thrown and propagate them to the caller.
                HttpWebResponseWrapper responseContent = await sendTask.ConfigureAwait(false);
                return responseContent;

            }            
            finally
            {
                Interlocked.Exchange(ref this.isSending, 0);
            }
        }

        public virtual HttpWebResponseWrapper Send()
        {
            if (Interlocked.CompareExchange(ref this.isSending, 1, 0) != 0)
            {
                throw new InvalidOperationException("Transmission is already in progress.");
            }
            try
            {
                try
                {
                    WebRequest request = this.CreateRequest(this.EndpointAddress);
                    request.Timeout = (int)this.Timeout.TotalMilliseconds;
                    return this.GetResponse(request);
                }
                catch (WebException ex)
                {
                    string str = string.Empty;
                    if (ex.Response != null)
                    {
                        using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                        {
                            str = reader.ReadToEnd();
                        }
                        ex.Response.Close();
                    }
                    if (ex.Status == WebExceptionStatus.Timeout)
                    {
                        throw new Exception(
                                "The timeout elapsed while attempting to transmit telemetry.", ex);

                    }
                    throw new Exception(String.Format(CultureInfo.InvariantCulture,
                        "A Web exception occurred while attempting to issue the request. {0}: {1}",
                        ex.Message, str), ex);
                }
            }            
            finally
            {
                Interlocked.Exchange(ref this.isSending, 0);
            }
        }
      
        protected virtual WebRequest CreateRequest(Uri address)
        {
            var request = WebRequest.Create(address);

            request.Method = "POST";

            if (!string.IsNullOrEmpty(this.ContentType))
            {
                request.ContentType = this.ContentType;
            }

            if (!string.IsNullOrEmpty(this.ContentEncoding))
            {
                request.Headers[ContentEncodingHeader] = this.ContentEncoding;
            }

            return request;
        }

        private async Task<HttpWebResponseWrapper> GetResponseAsync(WebRequest request)
        {
            using (Stream requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
            {
                await requestStream.WriteAsync(this.Content, 0, this.Content.Length).ConfigureAwait(false);
            }

            using (WebResponse response = await request.GetResponseAsync().ConfigureAwait(false))
            {
                return this.CheckResponse(response);
            }
        }

        private HttpWebResponseWrapper GetResponse(WebRequest request)
        {
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(this.Content, 0, this.Content.Length);
            }

            using (WebResponse response = request.GetResponse())
            {
                return this.CheckResponse(response);
            }
        }

        private HttpWebResponseWrapper CheckResponse(WebResponse response)
        {
            HttpWebResponseWrapper wrapper = null;

            var httpResponse = response as HttpWebResponse;
            if (httpResponse != null)
            {
                // Return content only for 206 for performance reasons
                // Currently we do not need it in other cases
                if (httpResponse.StatusCode == HttpStatusCode.PartialContent)
                {
                    wrapper = new HttpWebResponseWrapper
                    {
                        StatusCode = (int)httpResponse.StatusCode,
                        StatusDescription = httpResponse.StatusDescription
                    };

                    if (httpResponse.Headers != null)
                    {
                        wrapper.RetryAfterHeader = httpResponse.Headers["Retry-After"];
                    }

                    using (StreamReader content = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        wrapper.Content = content.ReadToEnd();
                    }
                }
            }

            return wrapper;
        }
    }
}
