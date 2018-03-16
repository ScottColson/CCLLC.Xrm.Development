using System.Xml.Linq;

namespace D365.XrmPluginExtensions.Configuration
{
    public interface IXmlConfigurationResource
    {
        XDocument Get(string key);

        XDocument Get(string key, bool disableCache);
    }
}
