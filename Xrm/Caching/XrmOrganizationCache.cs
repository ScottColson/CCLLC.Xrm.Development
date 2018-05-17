using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using System.Collections.Specialized;

namespace CCLCC.Xrm.Sdk.Caching
{

    /// <summary>
    /// Wraps the default .NET memory cache to provide a cache that is unique to and shared across a given 
    /// CRM organization within the scope of the hosting server. 
    /// </summary>
    public class XrmOrganizationCache : IXrmCache
    {
        private static MemoryCache Cache
        {
            get { return MemoryCache.Default; }
        }

        public Guid OrganizationId { get; private set; }


        internal XrmOrganizationCache(Guid OrganizationId)
        {
            this.OrganizationId = OrganizationId;
        }
        
        private string getOrganizationKey(string key)
        {
            const string keyFormat = "XrmMemoryCache.{1}.{0}";
            return string.Format(keyFormat, this.OrganizationId, key);
        }

        public void Add(string key, object data, int seconds)
        {
            CacheItemPolicy policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromSeconds(seconds) };
            Cache.Add(getOrganizationKey(key), data, policy);
        }

        public void Add<T>(string key, T data, int seconds)
        {
            Add(key, data, seconds);
        }

        public object Get(string key)
        {
            var orgKey = getOrganizationKey( key);
            return Cache.Get(orgKey);           
        }
        public T Get<T>(string key)
        {
            return (T)Get(key);
        }
           
        public void Remove(string key)
        {
            Cache.Remove(getOrganizationKey(key));
        }        

        public bool Exists(string key)
        {
            return Cache[getOrganizationKey(key)] != null;
        }

       
    }

}
