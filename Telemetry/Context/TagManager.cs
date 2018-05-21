using System;
using System.Collections.Generic;
using System.Globalization;
using CCLLC.Telemetry.Implementation;

namespace CCLLC.Telemetry.Context
{
   
    internal static class TagManager
    {
        internal static void SetStringValueOrRemove(this IDictionary<string, string> tags, string tagKey, string tagValue)
        {
            SetTagValueOrRemove(tags, tagKey, tagValue);
        }

        internal static void SetTagValueOrRemove<T>(this IDictionary<string, string> tags, string tagKey, T tagValue)
        {
            SetTagValueOrRemove(tags, tagKey, Convert.ToString(tagValue, CultureInfo.InvariantCulture));
        }

        internal static void CopyTagValue(bool? sourceValue, bool? targetValue)
        {
            if (sourceValue.HasValue && !targetValue.HasValue)
            {
                targetValue = sourceValue;
            }
        }

        internal static string GetTagValueOrNull(this IDictionary<string, string> tags, string tagKey)
        {
            string tagValue;
            if (tags.TryGetValue(tagKey, out tagValue))
            {
                return tagValue;
            }

            return null;
        }

        internal static void UpdateTagValue(this IDictionary<string, string> tags, string tagKey, string tagValue, IReadOnlyDictionary<string, int> tagSizeLimits)
        {
            if (!string.IsNullOrEmpty(tagValue) && !string.IsNullOrEmpty(tagKey))
            {
                int limit;
                if (tagSizeLimits != null && tagSizeLimits.TryGetValue(tagKey, out limit) && tagValue.Length > limit)
                {
                    tagValue = Property.TrimAndTruncate(tagValue, limit);
                }

                tags.Add(tagKey, tagValue);
            }
        }

        internal static void UpdateTagValue(this IDictionary<string, string> tags, string tagKey, bool? tagValue)
        {
            if (tagValue.HasValue && !string.IsNullOrEmpty(tagKey))
            {
                tags.Add(tagKey, tagValue.Value.ToString());
            }
        }

        internal static void CopyTagValue(string sourceValue, string targetValue)
        {
            if (!string.IsNullOrEmpty(sourceValue))
            {
                targetValue = sourceValue;
            }
        }

        

        private static void SetTagValueOrRemove(this IDictionary<string, string> tags, string tagKey, string tagValue)
        {
            if (string.IsNullOrEmpty(tagValue))
            {
                tags.Remove(tagKey);
            }
            else
            {
                tags[tagKey] = tagValue;
            }
        }
    }
}
