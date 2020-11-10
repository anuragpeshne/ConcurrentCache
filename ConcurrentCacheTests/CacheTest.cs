using ConcurrentCacheNS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

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

        [TestMethod]
        public void SingleThreaded_CacheHits()
        {
            int cacheSize = 1000;
            ConcurrentCache cache = new ConcurrentCache(cacheSize);
            foreach(var i in Enumerable.Range(0, cacheSize / 2))  cache.Get(i.ToString());
            Assert.AreEqual(0, cache.CacheHit);
            Assert.AreEqual(cacheSize / 2, cache.TotalRequest);

            foreach(var i in Enumerable.Range(0, cacheSize / 2)) cache.Get(i.ToString());
            Assert.AreEqual(cacheSize / 2, cache.CacheHit);
            Assert.AreEqual(cacheSize, cache.TotalRequest);

            foreach(var i in Enumerable.Range(cacheSize / 2, cacheSize / 2)) cache.Get(i.ToString());
            Assert.AreEqual(cacheSize / 2, cache.CacheHit);
            Assert.AreEqual(cacheSize * 3 / 2, cache.TotalRequest);

            foreach(var i in Enumerable.Range(0, cacheSize)) cache.Get(i.ToString());
            Assert.AreEqual(cacheSize * 3 / 2, cache.CacheHit);
            Assert.AreEqual(cacheSize * 5 / 2, cache.TotalRequest);
        }

        [TestMethod]
        public void MultiThreaded_CacheHits()
        {
            int cacheSize = 1000;
            ConcurrentCache cache = new ConcurrentCache(cacheSize);
            Parallel.ForEach(Enumerable.Range(0, cacheSize / 2), (i) => cache.Get(i.ToString()));
            Assert.AreEqual(0, cache.CacheHit);
            Assert.AreEqual(cacheSize / 2, cache.TotalRequest);

            Parallel.ForEach(Enumerable.Range(0, cacheSize / 2), (i) => cache.Get(i.ToString()));
            Assert.AreEqual(cacheSize / 2, cache.CacheHit);
            Assert.AreEqual(cacheSize, cache.TotalRequest);

            Parallel.ForEach(Enumerable.Range(cacheSize / 2, cacheSize / 2), (i) => cache.Get(i.ToString()));
            Assert.AreEqual(cacheSize / 2, cache.CacheHit);
            Assert.AreEqual(cacheSize * 3 / 2, cache.TotalRequest);

            Parallel.ForEach(Enumerable.Range(0, cacheSize), (i) => cache.Get(i.ToString()));
            Assert.AreEqual(cacheSize * 3 / 2, cache.CacheHit);
            Assert.AreEqual(cacheSize * 5 / 2, cache.TotalRequest);
        }
    }
}
