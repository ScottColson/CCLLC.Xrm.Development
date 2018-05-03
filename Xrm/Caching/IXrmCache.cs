
namespace CCLCC.Xrm.Caching
{
    public interface IXrmCache
    {
        void Add(string key, object data, int seconds);

        void Add<T>(string key, T data, int seconds);
        
        object Get(string key);

        T Get<T>(string key);

        bool Exists(string key);

        void Remove(string key);
    }
}
