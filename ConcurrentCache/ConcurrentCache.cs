using ConcurrentCacheNS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ConcurrentCacheNS
{
    public class ConcurrentCache
    {
        public const int DEFAUL_MAX_SIZE = 500;

        // for validation, measuring perf
        public int CacheHit;
        public int TotalRequest;

        private int maxSize;
        private ConcurrentDictionary<string, string> cacheMap;
        private IEvictionPolicy evictionPolicy;

        public ConcurrentCache(int size = DEFAUL_MAX_SIZE)
        {
            maxSize = size;
            cacheMap = new ConcurrentDictionary<string, string>();
            evictionPolicy = new LRUPolicy();
        }

        public string Get(string key)
        {
            bool cacheHit = true;
            string value = cacheMap.GetOrAdd(key, (key) =>
            {
                cacheHit = false;
                return GetFromDb(key);
            });

            if (cacheHit)
            {
                Interlocked.Increment(ref CacheHit);
            }
            else
            {
                if (cacheMap.Count > maxSize)
                {
                    var keyToEvict = evictionPolicy.GetKeyToEvict();
                    cacheMap.TryRemove(keyToEvict, out _);
                }
            }
            evictionPolicy.RegisterKeyHit(key);
            Interlocked.Increment(ref TotalRequest);
            return value;
        }

        private string GetFromDb(string key)
        {
            return key + "-value";
        }
    }
}
