using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Clutch.Runtime.Caching
{
    public sealed class MemoryCache : IDisposable
    {
        public MemoryCache()
        {
            poolingTimer = new Timer(PerformPooling, null, 1000, 10 * 1000);
            defaultRegion = new MemoryCacheRegion();
            regions = new Dictionary<string, MemoryCacheRegion>();
        }

        private Timer poolingTimer;
        private MemoryCacheRegion defaultRegion;
        private Dictionary<string, MemoryCacheRegion> regions;

        public readonly ReaderWriterLockSlim SyncRoot = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private void PerformPooling(object state)
        {
            SyncRoot.EnterWriteLock();
            try
            {
                defaultRegion.Cleanup();
                foreach (var region in regions.Values)
                    region.Cleanup();
            }
            catch (Exception /*ex*/)
            {
                //logger.WarnException("Cache pooling failed", ex);
            }
            finally
            {
                SyncRoot.ExitWriteLock();
            }
        }

        private MemoryCacheRegion ObtainRegion(string name, bool create = false)
        {
            if (name == null)
                return defaultRegion;

            MemoryCacheRegion region = null;

            if (!regions.TryGetValue(name, out region) && create)
                regions[name] = region = new MemoryCacheRegion();

            return region;
        }

        public void Set(string key, object value, DateTime expires, string region = null, CacheEntryExpiredCallback expireCallback = null)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            SyncRoot.EnterWriteLock();
            try
            {
                var store = ObtainRegion(region, create: true);

                store.Set(new CacheEntry(key, value, expires, expireCallback));
            }
            finally
            {
                SyncRoot.ExitWriteLock();
            }
        }

        public object Get(string key, string region = null)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            SyncRoot.EnterReadLock();
            try
            {
                var store = ObtainRegion(region);
                if (store == null)
                    return null;

                var entry = store.Get(key);
                if (entry == null)
                    return null;

                return entry.Value;
            }
            finally
            {
                SyncRoot.ExitReadLock();
            }
        }

        public void Remove(string key, string region = null)
        {
            SyncRoot.EnterWriteLock();
            try
            {
                var store = ObtainRegion(region);

                store.Remove(key);

                if (!store.Any())
                    regions.Remove(region);
            }
            finally
            {
                SyncRoot.ExitWriteLock();
            }
        }

        public void RemoveAll(string key)
        {
            SyncRoot.EnterWriteLock();
            try
            {
                defaultRegion.Remove(key);

                foreach (var store in regions.Values)
                    store.Remove(key);

                foreach (var region in regions.Where(p => !p.Value.Any()).Select(p => p.Key).ToArray())
                    regions.Remove(region);
            }
            finally
            {
                SyncRoot.ExitWriteLock();
            }
        }

        #region IDisposable

        public void Dispose()
        {
            poolingTimer.Dispose();
        }

        #endregion
    }
}
