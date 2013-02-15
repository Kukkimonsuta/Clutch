using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch.Runtime.Caching
{
    public sealed class CacheEntry
    {
        internal CacheEntry(string key, object value, DateTime expires, CacheEntryExpiredCallback expireCallback = null)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            Key = key;
            Value = value;
            Expires = expires;
            ExpireCallback = expireCallback;
        }

        private object value;

        public string Key { get; private set; }
        public DateTime Expires { get; set; }
        public object Value
        {
            get { return value; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.value = value;
            }
        }
        public CacheEntryExpiredCallback ExpireCallback { get; set; }

        internal bool IsExpired(DateTime now) { return now > Expires; }
    }

    public delegate void CacheEntryExpiredCallback(CacheEntry cache);
}
