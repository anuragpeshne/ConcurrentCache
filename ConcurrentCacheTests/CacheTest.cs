using ConcurrentCacheNS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcurrentCacheTests
{
    [TestClass]
    public class CacheTest
    {
        [TestMethod]
        public void SingleThreaded_EmptyCacheReturns()
        {
            ConcurrentCache cache = new ConcurrentCache(10);
            var value = cache.Get("1");
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SingleThreaded_RepeatedCallReturnsCachedValue()
        {
            ConcurrentCache cache = new ConcurrentCache(10);
            var val = cache.Get("1");
            var val2 = cache.Get("1");
            Assert.AreEqual(val, val2);
            Assert.AreEqual(1, cache.CacheHit);
        }

        [TestMethod]
        public void SingleThreaded_AutoEvictKeys()
        {
            int cacheSize = 5;
            ConcurrentCache cache = new ConcurrentCache(cacheSize);
            for (int i = 0; i < 2 * cacheSize; i++)
            {
                cache.Get(i.ToString());
            }
            // [5, 6, 7, 8, 9]
            for (int i = 0; i < cacheSize; i++)
            {
                cache.Get(i.ToString());
            }
            // [1, 2, 3, 4, 5]
            Assert.AreEqual(0, cache.CacheHit);
            Assert.AreEqual(3 * cacheSize, cache.TotalRequest);

            for (int i = 0; i < cacheSize; i++)
            {
                cache.Get(i.ToString());
            }
            // [1, 2, 3, 4, 5]
            Assert.AreEqual(cacheSize, cache.CacheHit);
        }
    }
}
