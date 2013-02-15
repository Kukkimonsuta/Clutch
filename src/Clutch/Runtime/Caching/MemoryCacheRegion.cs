using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch.Runtime.Caching
{
    internal class MemoryCacheRegion : IEnumerable<CacheEntry>
    {
        public MemoryCacheRegion()
        {
            store = new Dictionary<string, CacheEntry>();
        }

        private Dictionary<string, CacheEntry> store;

        public CacheEntry Get(string key)
        {
            if (!store.ContainsKey(key))
                return null;

            return store[key];
        }

        public void Set(CacheEntry entry)
        {
            store[entry.Key] = entry;
        }

        public void Remove(string key)
        {
            store.Remove(key);
        }

        public void Cleanup()
        {
            var now = DateTime.Now;
            var bucket = store.Where(p => p.Value.IsExpired(now)).ToArray();

            foreach (var item in bucket)
            {
                // call expire callback
                if (item.Value.ExpireCallback != null)
                    item.Value.ExpireCallback(item.Value);

                // if there was callback, item might have been revalidated
                if (item.Value.IsExpired(now))
                    store.Remove(item.Key);
            }
        }

        #region IEnumerable

        public IEnumerator<CacheEntry> GetEnumerator()
        {
            return store.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
