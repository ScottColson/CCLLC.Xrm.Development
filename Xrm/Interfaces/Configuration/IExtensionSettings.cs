
namespace CCLCC.Xrm.Sdk
{
    public interface IExtensionSettings
    {
        T Get<T>(string key, T defaultValue);

        void Update(string key, string value);

        void ClearCache();
    }
}
