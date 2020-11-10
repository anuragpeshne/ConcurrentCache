using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentCacheNS
{
    public class LRUPolicy : IEvictionPolicy
    {
        private LinkedList<string> lruList;
        private Dictionary<string, LinkedListNode<string>> lruMap;

        private readonly object lockObj = new object();

        public LRUPolicy()
        {
            lruList = new LinkedList<string>();
            lruMap = new Dictionary<string, LinkedListNode<string>>();
        }

        public string GetKeyToEvict()
        {
            // This method will be called on cache miss.
            // We expect this to happen less frequently,
            // => okay to block all the threads trying to get tail
            string key;
            lock (lockObj)
            {
                var node = lruList.Last;
                lruList.RemoveLast();
                lruMap.Remove(node.Value);
                key = node.Value;
            }
            return key;
        }

        public void RegisterKeyHit(string key)
        {
            MoveKeyToFront(key);
        }

        private void MoveKeyToFront(string key)
        {
            // We should expect a lot of calls for this method
            lock(lockObj)
            {
                LinkedListNode<string> node;
                if (lruMap.ContainsKey(key))
                {
                    node = lruMap[key];
                    lruList.Remove(node);
                }
                else
                {
                    node = new LinkedListNode<string>(key);
                    lruMap[key] = node;
                }
                lruList.AddFirst(node);
            }
        }
    }
}
