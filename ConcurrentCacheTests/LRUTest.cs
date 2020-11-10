using ConcurrentCacheNS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcurrentCacheTests
{
    [TestClass]
    public class LRUTest
    {
        [TestMethod]
        public void SingleThreaded_KeyToEvict()
        {
            IEvictionPolicy policy = new LRUPolicy();
            for (int i = 0; i < 5; i++)
            {
                policy.RegisterKeyHit(i.ToString());
            }
            // Currently in list [0, 1, 2, 3, 4]
            Assert.AreEqual("0", policy.GetKeyToEvict());
            policy.RegisterKeyHit("5");
            // [1, 2, 3, 4, 5]
            policy.RegisterKeyHit("1");
            // [2, 3, 4, 5, 1]
            Assert.AreEqual("2", policy.GetKeyToEvict());
            policy.RegisterKeyHit("6");
            // [3, 4, 5, 1, 6]
            policy.RegisterKeyHit("6");
            // [3, 4, 5, 1, 6]
            Assert.AreEqual("3", policy.GetKeyToEvict());
            // [4, 5, 1, 6]
            Assert.AreEqual("4", policy.GetKeyToEvict());
            // [5, 1, 6]
            Assert.AreEqual("5", policy.GetKeyToEvict());
            // [1, 6]
            Assert.AreEqual("1", policy.GetKeyToEvict());
            // [6]
            Assert.AreEqual("6", policy.GetKeyToEvict());
        }
    }
}
