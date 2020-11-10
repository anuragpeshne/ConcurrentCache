namespace ConcurrentCacheNS
{
    public interface IEvictionPolicy
    {
        public string GetKeyToEvict();
        public void RegisterKeyHit(string key);
    }
}
