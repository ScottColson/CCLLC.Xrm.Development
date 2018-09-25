using System;
using System.Runtime.Caching;

namespace CCLLC.Xrm.Sdk.Caching
{

    /// <summary>
    /// Wraps the default <see cref="MemoryCache"/> to provide a cache that is unique to and shared across a given 
    /// CRM organization within the scope of the hosting process. 
    /// </summary>
    public class XrmOrganizationCache : IXrmCache
    {
        private static MemoryCache Cache
        {
            get { return MemoryCache.Default; }
        }

        public Guid OrganizationId { get; private set; }


        protected internal XrmOrganizationCache(Guid OrganizationId)
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
            if(seconds < 0) { seconds = 0; }
            this.Add(key, data, TimeSpan.FromSeconds(seconds));            
        }

        public void Add(string key, object data, TimeSpan lifetime)
        {
            if(lifetime == default(TimeSpan)) { lifetime = new TimeSpan(0, 5, 0); }
            CacheItemPolicy policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + lifetime };
            Cache.Add(getOrganizationKey(key), data, policy);
        }

        public void Add<T>(string key, T data, int seconds)
        {
            if (seconds < 0) { seconds = 0; }
            this.Add<T>(key, data, TimeSpan.FromSeconds(seconds));
        }

        public void Add<T>(string key, T data, TimeSpan lifetime)
        {
            if (lifetime == default(TimeSpan)) { lifetime = new TimeSpan(0, 5, 0); }
            CacheItemPolicy policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + lifetime };
            Cache.Add(getOrganizationKey(key), data, policy);
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
