using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.ServiceModel.Configuration;
using Microsoft.Crm.Sdk.Messages;

namespace CCLCC.XrmPluginExtensions.Configuration
{
    using Caching;
    using Encryption;

    public class ExtensionSettings : IExtensionSettings
    {
        const int DEFAULT_CACHE_TIMEOUT = 1800; //30 minutes
        const int MAX_CACHE_TIMEOUT = 28800; //8 hours
        const int MIN_CACHE_TIMEOUT = 0; //no cache

        const string CACHE_ENTRIES_KEY = "CCLCC.XrmPluginExtensions.ExtensionSettings.Entries.Dictionary";
        const string CACHE_SETTING_NAME = "CacheTimeOut";
        const string DEFAULT_ENCRYPTION_KEY = "7a5a64brEgaceqenuyegac7era3Ape6aWatrewegeka94waqegayathudrebuc7t";

        private static object syncRoot = new Object();

        private IOrganizationService orgService;
        private IXrmCache cache;
        private IEncryption encryption;
        private string encryptionKey;

        char[] SEPARATORS = { ';', ',' };

        public ExtensionSettings(IOrganizationService OrgService, IXrmCache cache, IEncryption encryption, string key = null)
        {
            this.orgService = OrgService;
            this.cache = cache;
            this.encryption = encryption;
            encryptionKey = !string.IsNullOrEmpty(key) ? key : DEFAULT_ENCRYPTION_KEY;
        }

        private int GetCacheTimeout(Dictionary<string, string> entries)
        {
            //Check settings for custom cache timeout setting otherwise use the default
            int expireSeconds;
            string val;

            if (entries.TryGetValue(CACHE_SETTING_NAME.ToLower(), out val))
            {
                expireSeconds = System.Convert.ToInt32(val);
                if (expireSeconds > MAX_CACHE_TIMEOUT)
                {
                    expireSeconds = MAX_CACHE_TIMEOUT;
                }
                else if (expireSeconds < MIN_CACHE_TIMEOUT)
                {
                    expireSeconds = MIN_CACHE_TIMEOUT;
                }
            }
            else
                expireSeconds = DEFAULT_CACHE_TIMEOUT;

            return expireSeconds;
        }

        private Dictionary<string, string> RetrieveCurrentSettings()
        {
            //lock access while refresh is being completed.
            lock (syncRoot)
            {
                //reload the cache
                try
                {
                    //query the system for all active extension settings
                    QueryByAttribute qry = new QueryByAttribute("dmvm_extensionsetting");
                    qry.ColumnSet = new ColumnSet(new string[] { "dmvm_name", "dmvm_value", "dmvm_encrypted" });
                    qry.AddAttributeValue("statecode", 0);

                    EntityCollection result = this.orgService.RetrieveMultiple(qry);
                    Dictionary<string, string> entries = new Dictionary<string, string>(result.Entities.Count);

                    foreach (Entity setting in result.Entities)
                    {
                        var name = setting.GetAttributeValue<string>("dmvm_name");
                        name = !string.IsNullOrEmpty(name) ? name.ToLowerInvariant() : string.Empty;
                        var value = setting.GetAttributeValue<string>("dmvm_value");

                        bool encrypted = setting.GetAttributeValue<bool>("dmvm_encrypted");

                        if (encrypted)
                        {
                            value = encryption.Decrypt(value, encryptionKey);
                        }
                        entries.Add(name, value);
                    }
                    return entries;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error retrieving extension settings: {0}", ex.Message));
                }

            } //end lock
        }

        public T Get<T>(string Key, T DefaultValue)
        {
            string value;
            var entries = cache.Get(CACHE_ENTRIES_KEY) as Dictionary<string, string>;
            if (entries == null)
            {
                entries = RetrieveCurrentSettings();
                if (entries == null)
                    throw new Exception("The cache is empty after being refreshed");
                int seconds = GetCacheTimeout(entries);
                cache.Add(CACHE_ENTRIES_KEY, entries, seconds);
            }

            if (entries.TryGetValue(Key.ToLower(), out value))
            {
                if (typeof(T) != typeof(string[]))
                    return (T)((object)Convert.ChangeType(value, typeof(T)));
                else
                {
                    //create a string array to return
                    string[] strArray = new string[0];
                    string stringValue = value.ToString();

                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        //remove any white space following the seperator.
                        value = System.Text.RegularExpressions.Regex.Replace(value, @";\s+", ";");

                        //split the exlusion field string into an array
                        strArray = value.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
                    }

                    return (T)((object)strArray);
                }
            }
            else
                return DefaultValue;

        }

        public void Update(string key, string value)
        {
            var qry = new QueryExpression
            {
                EntityName = "dmvm_extensionsetting",
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "dmvm_name",
                            Operator = ConditionOperator.Equal,
                            Values = { key}
                        }
                    }
                }
            };

            var results = orgService.RetrieveMultiple(qry);

            if (results.Entities.Count == 0)
            {
                var record = new Entity("dmvm_extensionsetting");
                record["dmvm_value"] = value;
                record["dmvm_name"] = key;
                this.orgService.Create(record);
            }
            else
            {
                var record = new Entity("dmvm_extensionsetting", results.Entities[0].Id);
                record["dmvm_value"] = value;
                orgService.Update(record);
            }

            this.ClearCache();
        }

        public void ClearCache()
        {
            cache.Remove(CACHE_ENTRIES_KEY);
        }

    }
}
