using System;
using System.Collections.Generic;

namespace CCLLC.Xrm.Sdk.Caching
{
    internal class CacheItem
    {
        public DateTime ExpiresOn { get; private set; }
        public object value { get; private set; }

        public CacheItem(object value, int timeout)
        {
            this.value = value;
            this.ExpiresOn = DateTime.UtcNow.AddSeconds(timeout);
        }
    }

    public class XrmPluginCache : IXrmCache
    {
        const int DEFAULT_CACHE_TIMEOUT = 300; //5 minutes
        const int MAX_CACHE_TIMEOUT = 43200; //12 hours
        const int MIN_CACHE_TIMEOUT = 0; //no caching

        private static XrmPluginCache instance;
        private static object syncRoot = new object();
        private static volatile Dictionary<string, CacheItem> _cache;

        protected internal XrmPluginCache()
        {
            _cache = new Dictionary<string, CacheItem>();
        }

        internal protected static XrmPluginCache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock(syncRoot)
                    {
                        if(instance == null)
                            instance = new XrmPluginCache();
                    }
                }

                return instance;
            }
        }

        public bool Exists(string key)
        {
            if (_cache.ContainsKey(key))
            {
                if (_cache[key].ExpiresOn > DateTime.UtcNow)
                {
                    return true;
                }

                //cached value is expired so remove it now.
                this.Remove(key);
            }

            return false;
        }

        public void Add(string key, object data, TimeSpan lifetime)
        {
            if (lifetime == default(TimeSpan))
            {
                this.Add(key, data);
            }
            else
            {
                this.Add(key, data, (int)lifetime.TotalSeconds);
            }
        }

        public void Add(string key, object data, int seconds = DEFAULT_CACHE_TIMEOUT)
        {
            if (seconds < MIN_CACHE_TIMEOUT)
            {
                seconds = MIN_CACHE_TIMEOUT;
            }
            else if (seconds > MAX_CACHE_TIMEOUT)
            {
                seconds = MAX_CACHE_TIMEOUT;
            }

            CacheItem item = new CacheItem(data, seconds);

            lock (syncRoot)
            {
                if (_cache.ContainsKey(key))
                {
                    _cache[key] = item;
                }
                else
                {
                    _cache.Add(key, item);
                }
            }
        }

        public void Add<T>(string key, T data, TimeSpan lifetime)
        {
            if (lifetime == default(TimeSpan))
            {
                this.Add<T>(key, data);
            }
            else
            {
                this.Add<T>(key, data, (int)lifetime.TotalSeconds);
            }
        }

        public void Add<T>(string key, T value, int seconds = DEFAULT_CACHE_TIMEOUT)
        {

            if(seconds < MIN_CACHE_TIMEOUT)
            {
                seconds = MIN_CACHE_TIMEOUT;
            }
            else if(seconds > MAX_CACHE_TIMEOUT)
            {
                seconds = MAX_CACHE_TIMEOUT;
            }

            CacheItem item = new CacheItem(value, seconds);

            lock (syncRoot)
            {
                if (_cache.ContainsKey(key))
                {
                    _cache[key] = item;
                }
                else
                {
                    _cache.Add(key, item);
                }
            }
        }

        public T Get<T>(string key)
        {
            if (this.Exists(key))
            {
                if (_cache[key].ExpiresOn >=  DateTime.UtcNow)
                {
                    object value = _cache[key].value;
                    
                    //return (T)((object)Convert.ChangeType(value, typeof(T)));
                    return (T)value;
                }
            }

            return default(T);
        }

        public void Remove(string key)
        {
            lock (syncRoot)
            {
                if (_cache.ContainsKey(key))
                {
                    _cache.Remove(key);
                }
            }
        }

       

        public object Get(string key)
        {
            if (this.Exists(key))
            {
                if (_cache[key].ExpiresOn >= DateTime.UtcNow)
                {
                    object value = _cache[key].value;

                    return value;
                }
            }

            return null;
        }
    }
}
