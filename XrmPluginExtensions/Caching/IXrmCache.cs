using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.XrmPluginExtensions.Caching
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
