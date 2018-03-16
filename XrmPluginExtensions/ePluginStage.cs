using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.XrmPluginExtensions
{
    public enum ePluginStage
    {    
        PreValidation = 10,
        PreOperation = 20,
        PostOperation = 40
    }
}
