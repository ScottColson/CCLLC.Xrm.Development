﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry
{
    public interface IPageViewTelemetry : ITelemetry, IDataModelTelemetry<IPageViewDataModel>
    {
    }
}