using System.Xml.Linq;

namespace CCLCC.XrmBase.Configuration
{
    public interface IXmlConfigurationResource
    {
        XDocument Get(string key);

        XDocument Get(string key, bool disableCache);
    }
}
