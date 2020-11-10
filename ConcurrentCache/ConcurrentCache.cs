using ConcurrentCacheNS;
using System;
using System.Collections.Generic;

namespace ConcurrentCacheNS
{
    public class ConcurrentCache
    {
        public const int DEFAUL_MAX_SIZE = 500;

        // for validation, measuring perf
        public int CacheHit { get; private set; }
        public int TotalRequest { get; private set; }

        private int maxSize;
        private Dictionary<string, string> cacheMap;
        private IEvictionPolicy evictionPolicy;

        public ConcurrentCache(int size = DEFAUL_MAX_SIZE)
        {
            maxSize = size;
            cacheMap = new Dictionary<string, string>();
            evictionPolicy = new LRUPolicy(size);
        }

        public string Get(string key)
        {
            string value;
            if (cacheMap.ContainsKey(key))
            {
                value = cacheMap[key];
                CacheHit++;
            }
            else
            {
                value = GetFromDb(key);
                if (cacheMap.Count > maxSize)
                {
                    var keyToEvict = evictionPolicy.GetKeyToEvict();
                    cacheMap.Remove(keyToEvict);
                }
                cacheMap[key] = value;
            }
            evictionPolicy.RegisterKeyHit(key);
            TotalRequest++;
            return value;
        }

        private string GetFromDb(string key)
        {
            return key + "-value";
        }
    }
}
