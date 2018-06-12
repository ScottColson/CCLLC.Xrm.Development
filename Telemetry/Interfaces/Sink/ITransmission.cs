using System;
using System.Threading.Tasks;

namespace CCLLC.Telemetry
{
    public interface ITransmission
    {      
        IHttpWebResponseWrapper Send(TimeSpan timeout = default(TimeSpan));
        Task<IHttpWebResponseWrapper> SendAsync(TimeSpan timeout = default(TimeSpan));
    }
}
