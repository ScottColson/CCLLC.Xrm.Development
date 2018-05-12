using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.Telemetry.Implementation
{
    internal static partial class Utils
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value == null)
            {
                return true;
            }
#if !NETSTANDARD1_3
            return value.All(char.IsWhiteSpace);
#else
            return string.IsNullOrWhiteSpace(value);
#endif
        }

        public static void CopyDictionary<TValue>(IDictionary<string, TValue> source, IDictionary<string, TValue> target)
        {
            foreach (KeyValuePair<string, TValue> pair in source)
            {
                if (string.IsNullOrEmpty(pair.Key))
                {
                    continue;
                }

                if (!target.ContainsKey(pair.Key))
                {
                    target[pair.Key] = pair.Value;
                }
            }
        }

        
        public static string PopulateRequiredStringValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {               
                return "n/a";
            }

            return value;
        }


        public static double SanitizeNanAndInfinity(double value)
        {
            bool valueChanged;
            return SanitizeNanAndInfinity(value, out valueChanged);
        }

        public static double SanitizeNanAndInfinity(double value, out bool valueChanged)
        {
            valueChanged = false;

            // Disallow Nan and Infinity since Breeze does not accept it
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                value = 0;
                valueChanged = true;
            }

            return value;
        }

        public static TimeSpan ValidateDuration(string value)
        {
            TimeSpan interval;
            if (!TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out interval))
            {                
                return TimeSpan.Zero;
            }

            return interval;
        }

    }
}
