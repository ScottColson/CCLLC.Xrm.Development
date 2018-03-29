using System.Linq;
using Microsoft.Xrm.Sdk;

namespace CCLCC.XrmBase.Utilities
{    
    
    /// <summary>
    /// Extensions for the Xrm Entity class to provide a set of common functions for manipulating Entity records.
    /// </summary>
    public static class XrmEntityExtension
    {
        /// <summary>
        /// Adds the specified attribute, or updates the value if the attribute exists.
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="AttributeName"></param>
        /// <param name="Value"></param>
        public static void AddUpdateAttribute(this Entity Target, string AttributeName, object Value)
        {
            if (Target.Attributes.Contains(AttributeName))
            {
                Target.Attributes[AttributeName] = Value;
            }
            else
            {
                Target.Attributes.Add(AttributeName, Value);
            }
        }


        /// <summary>
        /// Adds an attribute to the target entity by copying a specified attribute from a source entity. If 
        /// the target already contains the enitity then the target attribute will be updated.
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="TargetAttributeName"></param>
        /// <param name="Source"></param>
        /// <param name="SourceAttributeName"></param>
        public static void AddUpdateAttributeFromSource(this Entity Target, string TargetAttributeName, Entity Source, string SourceAttributeName)
        {
            if (Source.Contains(SourceAttributeName))
            {
                object value = Source[SourceAttributeName];
                Target.AddUpdateAttribute(TargetAttributeName, value);
            }
        }

        /// <summary>
        /// Checks the target for existance of any attribute contained in the provided array of attribute names and returns
        /// true if at least one of the provided attributes exists.
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="AttributeNames"></param>
        /// <returns></returns>
        public static bool ContainsAny(this Entity Target, string[] AttributeNames)
        {
            foreach (string a in AttributeNames)
            {
                if (Target.Contains(a))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Retrieves an attribute from the entity with of the specified type and returns a 
        /// default value if the attribute does not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Target"></param>
        /// <param name="Key"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(this Entity Target, string Key, T DefaultValue)
        {
            if (!Target.Contains(Key))
            {
                return DefaultValue;
            }
            else if (Target[Key] == null)
            {
                return DefaultValue;
            }
            else
            {
                return Target.GetAttributeValue<T>(Key);
            }
        }


        public static T GetAliasedValue<T>(this Entity Target, string EntityAlias, string FieldName, T DefaultValue)
        {
            string key = string.Format("{0}.{1}", EntityAlias, FieldName);
            if (!Target.Contains(key))
            {
                return DefaultValue;
            }
            else
            {
                AliasedValue value = Target.GetAttributeValue<AliasedValue>(key);
                return (T)value.Value;
            }
        }


        /// <summary>
        /// Merge the entity with another entity to get a more complete list of attributes. If 
        /// the current entity and the source entity both have a value for a given attribute 
        /// then then attribute of the current entity will be preserved.
        /// </summary>
        /// <param name="copyTo"></param>
        /// <param name="copyFrom"></param>
        public static void MergeWith(this Entity copyTo, Entity copyFrom)
        {
            if (copyTo != null && copyFrom != null)
            {
                copyFrom.Attributes.ToList().ForEach(a =>
                {
                    // if it already exists then dont copy
                    if (!copyTo.Attributes.ContainsKey(a.Key))
                    {
                        copyTo.Attributes.Add(a.Key, a.Value);
                    }
                });
            }
        }


        /// <summary>
        /// Removes an attribute from the entity attribute collection if that attribute exists.
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="AttributeName"></param>
        public static void RemoveAttribute(this Entity Target, string AttributeName)
        {
            if (Target.Contains(AttributeName))
            {
                Target.Attributes.Remove(AttributeName);
            }
        }
    }
}

