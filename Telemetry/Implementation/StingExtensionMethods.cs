using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Implementation
{
    internal static class StingExtensionMethods
    {
        internal static string TrimAndTruncate(this string value, int maxLength)
        {
            if (value != null)
            {
                return value.Trim().Truncate(maxLength);                
            }

            return value;
        }

        internal static string Truncate(this string value, int maxLength)
        {
            return value.Length > maxLength ? value.Substring(0, maxLength) : value;
        }
    }
}
