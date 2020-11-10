using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentCacheNS
{
    public class LRUPolicy : IEvictionPolicy
    {
        private LinkedList<string> lruList;
        private Dictionary<string, LinkedListNode<string>> lruMap;
        private int maxSize;

        public LRUPolicy(int size)
        {
            maxSize = size;
            lruList = new LinkedList<string>();
            lruMap = new Dictionary<string, LinkedListNode<string>>(size);
        }

        public string GetKeyToEvict()
        {
            var node = lruList.Last;
            lruList.RemoveLast();
            lruMap.Remove(node.Value);
            return node.Value;
        }

        public void RegisterKeyHit(string key)
        {
            MoveKeyToFront(key);
        }

        private void MoveKeyToFront(string key)
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
