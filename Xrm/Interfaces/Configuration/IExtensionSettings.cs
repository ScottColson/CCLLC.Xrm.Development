
namespace CCLCC.Xrm.Sdk
{
    public interface IExtensionSettings
    {
        T Get<T>(string key, T defaultValue = default(T));

        void Update(string key, string value);

        void ClearCache();
    }
}
