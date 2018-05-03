using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Xml.Linq;

namespace CCLCC.Xrm.Configuration
{
    using Caching;

    /// <summary>
    /// Retrieves XML Configuration Data stored in CRM Web Resources. Configuration data is cached for future use. The
    /// cache lifetime can be set for each Web Resource by adding a 'cacheTimeOutSetting' attribute to the root element
    /// of the Web Resource XML and setting the desired timeout in seconds.
    /// </summary>
    public class XmlConfigurationResource : IXmlConfigurationResource
    {
        private IOrganizationService orgService;
        private IXrmCache cache;

        internal XmlConfigurationResource(IOrganizationService orgService, IXrmCache cache)
        {
            this.orgService = orgService;
            this.cache = cache;
        }        
        
        /// <summary>
        /// Returns an XDocument representing XML configuration data stored in a named Web Resource. Retrieves 
        /// the XDocument from the XML Configuration data cache if it exists and has not expired.
        /// </summary>
        /// <param name="key">The name of the Web Resource to load.</param>
        /// <param name="disableCache">Flag indicating that cache should be ignored and values should come directly from the CRM database.</param>
        /// <returns>XDocument representing the XML data contained in the Web Resource.</returns>
        public  XDocument Get(string key, bool disableCache = false)
        { 
            return XmlConfigurationResourceCache.Instance.GetXmlConfigurationDoc(orgService, key, disableCache);
        }

        public XDocument Get(string key)
        {
            return this.Get(key, false);
        }

    }
    
    
    /// <summary>
    /// Retrieves and caches XML Configuration Data stored in CRM Web Resources. Each resource that is retrieved
    /// is cached for future use. The XmlConfigurationResourceCache is implemented as a Singleton so that each class
    /// within a givine plugin assembly shares a common instance and a common cache.
    /// </summary>
    class XmlConfigurationResourceCache
    {
        const int DEFAULT_CACHE_TIMEOUT = 1800; //30 minutes
        const int MAX_CACHE_TIMEOUT = 43200; //12 hours
        const int MIN_CACHE_TIMEOUT = 0; //0 seconds - cache disabled

        const string CACHE_TIMEOUT_ATTRIBUTE_NAME = "cacheTimeOutSetting";
        const string CACHE_EXPIRATION_DATE_ATTRIBUTE_NAME = "cacheExpirationDate";

        private static XmlConfigurationResourceCache instance;
        private static object syncRoot = new Object();

        private volatile Dictionary<string, XDocument> _values = new Dictionary<string, XDocument>();
        
        private XmlConfigurationResourceCache()
        {
        }

        public void ClearCache()
        {
            if (instance != null)
            {
                lock (syncRoot)
                {
                    instance._values.Clear();
                }
            }
        }

        public static XmlConfigurationResourceCache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new XmlConfigurationResourceCache();
                        }
                    }
                }

                return instance;
            }
        }

        public bool Contains(string resourceName)
        {
            return _values.ContainsKey(resourceName);
        }

        public XDocument GetXmlConfigurationDoc(IOrganizationService service, string resourceName, bool disableCache = false)
        {
            resourceName = resourceName.ToLower();

            //Get the item from the cache if it exists.
            if (!disableCache && _values.ContainsKey(resourceName))
            {
                XDocument cachedDoc = _values[resourceName];
                //get the expiration date of the cached content.
                DateTime cachedExpirationDate = DateTime.MinValue;
                if (cachedDoc.Root.Attribute(CACHE_EXPIRATION_DATE_ATTRIBUTE_NAME) != null)
                {
                    cachedExpirationDate = DateTime.Parse(cachedDoc.Root.Attribute(CACHE_EXPIRATION_DATE_ATTRIBUTE_NAME).Value).ToUniversalTime();
                }

                //if the item has not expired then return it now
                if (cachedExpirationDate > DateTime.UtcNow)
                {
                    return cachedDoc;
                }
            }


            //Valid copy of the configuration was not in the cache so load it from the 
            //CRM system now. Make sure only one thread is adding items to the cache at
            //any given time.
            QueryByAttribute qry = new QueryByAttribute
            {
                EntityName = "webresource",
                ColumnSet = new ColumnSet(true)
            };

            qry.AddAttributeValue("name", resourceName);

            EntityCollection results = service.RetrieveMultiple(qry);

            if (results.Entities.Count <= 0)
            {
                throw new InvalidPluginExecutionException(string.Format("Web resource '{0}' does not exist.",resourceName));
            }

            //convert the web resource from its native Base64 string format to xml.
            Entity resource = results.Entities[0];
            byte[] b = Convert.FromBase64String(resource.Attributes["content"].ToString());
            string xml = UnicodeEncoding.UTF8.GetString(b, 0, b.Length);

            //create an XDocument by parsing the xml.
            XDocument doc = XDocument.Parse(xml);

            if (!disableCache)
            {
                //calculate the cache timeout for this item. If the web resource has a cache timeout attribue, use it.
                int cacheTimeOut = DEFAULT_CACHE_TIMEOUT;
                if (doc.Root.Attribute(CACHE_TIMEOUT_ATTRIBUTE_NAME) != null)
                {
                    cacheTimeOut = int.Parse(doc.Root.Attribute(CACHE_TIMEOUT_ATTRIBUTE_NAME).Value);
                    if (cacheTimeOut < MIN_CACHE_TIMEOUT)
                    {
                        cacheTimeOut = MIN_CACHE_TIMEOUT;
                    }
                    else if (cacheTimeOut > MAX_CACHE_TIMEOUT)
                    {
                        cacheTimeOut = MAX_CACHE_TIMEOUT;
                    }
                }

                //make sure only one thread writes to the cache dictionary at any one time.
                lock (syncRoot)
                {
                    //add an attribute to the Xdoc to set the expiration for the configuration data.
                    DateTime expirationDate = DateTime.UtcNow.AddSeconds(cacheTimeOut);
                    doc.Root.SetAttributeValue(CACHE_EXPIRATION_DATE_ATTRIBUTE_NAME, expirationDate);

                    //add the Xdoc to the cache
                    if (_values.ContainsKey(resourceName))
                    {
                        _values[resourceName] = doc;
                    }
                    else
                    {
                        _values.Add(resourceName, doc);
                    }
                }
            }

            //return the Xdoc.
            return doc;
        }

    }

    
}
