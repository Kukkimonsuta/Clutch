using Clutch.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch.Web.Routing
{
    /// <summary>
    /// Base class for managing route token pairs
    /// </summary>
    public abstract class TokenCollection
    {
        private const string CACHE_ID = "{0}.id.{1}";
        private const string CACHE_TOKEN = "{0}.token.{1}";

        public TokenCollection()
        {
            instanceID = Guid.NewGuid().ToString();
        }

        private static readonly MemoryCache cache = new MemoryCache();

        private string instanceID;

        private void InsertIntoCache(int id, string token, string dependency)
        {
            var expires = DateTime.Now.AddHours(1);

            cache.SyncRoot.EnterWriteLock();
            try
            {
                cache.Set(string.Format(CACHE_ID, instanceID, id), token, expires, region: dependency);
                cache.Set(string.Format(CACHE_TOKEN, instanceID, token), id, expires, region: dependency);
            }
            finally
            {
                cache.SyncRoot.ExitWriteLock();
            }
        }

        protected abstract string Load(int id, string dependency);
        protected abstract int? Load(string token, string dependency);

        protected void Reset(int id, string dependency)
        {
            cache.SyncRoot.EnterWriteLock();
            try
            {
                var idKey = string.Format(CACHE_ID, instanceID, id);
                var token = cache.Get(idKey, region: dependency) as string;
                if (token == null)
                    return;

                cache.RemoveAll(idKey);
                cache.RemoveAll(string.Format(CACHE_TOKEN, instanceID, token));

                //logger.Trace("Force-removed '{0}':'{1}'", id, token);
            }
            finally
            {
                cache.SyncRoot.ExitWriteLock();
            }
        }

        /// <summary>
        /// Translates ID to token
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>Token or null</returns>
        public string Translate(int id, string dependency)
        {
            var token = cache.Get(string.Format(CACHE_ID, instanceID, id), region: dependency) as string;

            if (token == null)
            {
                token = Load(id, dependency);
                if (token != null)
                {
                    InsertIntoCache(id, token, dependency);
                    //logger.Trace("ID->Token: Loaded '{0}' -> '{1}' for dependency '{2}'", id, token, dependency);
                }
                //else
                //    logger.Trace("ID->Token: Failed to load '{0}' for dependency '{1}'", id, dependency);
            }
            //else
            //    logger.Trace("ID->Token: Translated '{0}' to '{1}' from cache for dependency '{2}'", id, token, dependency);

            return token;
        }

        /// <summary>
        /// Translates token to ID
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>ID or null</returns>
        public int? Translate(string token, string dependency)
        {
            var id = cache.Get(string.Format(CACHE_TOKEN, instanceID, token), region: dependency) as int?;

            if (id == null)
            {
                id = Load(token, dependency);
                if (id != null)
                {
                    InsertIntoCache(id.Value, token, dependency);
                    //logger.Trace("Token->ID: Loaded '{0}' -> '{1}' for dependency '{2}'", token, id);
                }
                //else
                //    logger.Trace("Token->ID: Failed to load '{0}' for dependency '{1}'", token, dependency);
            }
            //else
            //    logger.Trace("Token->ID: Translated '{0}' to '{1}' from cache for dependency '{2}'", token, id, dependency);

            return id;
        }
    }
}
