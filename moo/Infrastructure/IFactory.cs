namespace moo.Infrastructure
{
    public interface IFactory
    {
        public TValue GetService<TKey, TValue>(TKey key);
    }
}