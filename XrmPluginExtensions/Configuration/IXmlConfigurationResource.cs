using System.Xml.Linq;

namespace CCLCC.XrmPluginExtensions.Configuration
{
    public interface IXmlConfigurationResource
    {
        XDocument Get(string key);

        XDocument Get(string key, bool disableCache);
    }
}
