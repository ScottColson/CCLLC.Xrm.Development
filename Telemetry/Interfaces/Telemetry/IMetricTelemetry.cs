using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Interfaces
{
    public interface IMetricTelemetry : ITelemetry, IDataModelTelemetry<IMetricDataModel>, ISupportProperties, ISupportSampling
    {
    }
}
