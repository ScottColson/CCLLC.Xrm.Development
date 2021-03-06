﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using CCLLC.Xrm.Sdk.Caching;

namespace CCLLC.Xrm.Sdk.Utilities
{

   

    public class CachedEntities
    {
        private IOrganizationService OrganizationService { get; set; }
        private IXrmCache Cache { get; set; }

        public CachedEntities(IOrganizationService OrgService, IXrmCache cache)
        {
            this.OrganizationService = OrgService;
            this.Cache = cache;
        }

        /// <summary>
        /// Retrieves a record and caches it for future retrieval using this method.
        /// </summary> 
        /// <param name="entityName">The logical name of the entity that is being retrieved.</param>
        /// <param name="id">The record id of the record being retrieved.</param>
        /// <param name="columnSet">The columns that are being retrieved.</param>
        /// <param name="seconds">The amount of time to cache the results in seconds.</param>
        /// <param name="CacheKey">Unique key used to group queries. If null the system will compute a key</param>
        /// <returns></returns>
        public Entity GetCachedEntity(EntityReference reference, ColumnSet columnSet, int seconds = 1800, string CacheKey = null)
        {
            if (CacheKey == null)
            {
                CacheKey = string.Empty;
            }

            //generate a unique key by concatinating the cachekey, record type, and record guid.
            string key = "ENTITYCACHE_" + CacheKey + "_" + reference.LogicalName + reference.Id.ToString();

            //get the record from the cache if it iexists
            Entity record = Cache.Get<Entity>(key);

            if (record == null)
            {
                record = OrganizationService.Retrieve(reference.LogicalName, reference.Id, columnSet);
                if (record != null)
                {
                    Cache.Add<Entity>(key, record, seconds);
                }
            }

            return record;
        }

        public T GetCachedEntity<T>(EntityReference reference, ColumnSet columnSet, int timeout = 1800, string CacheKey = null) where T : Entity
        {
            if (CacheKey == null)
            {
                CacheKey = string.Empty;
            }

            //generate a unique key by concatinating the cachekey, record type, and record guid.
            string key = "ENTITYCACHE_" + CacheKey + "_" + reference.LogicalName + reference.Id.ToString();

            //get the record from the cache if it iexists
            var record = Cache.Get<T>(key);

            if (record == null)
            {
                record = OrganizationService.Retrieve(reference.LogicalName, reference.Id, columnSet).ToEntity<T>();
                if (record != null)
                {
                    Cache.Add<T>(key, record, timeout);
                }
            }

            return record;
        }

    }
}
