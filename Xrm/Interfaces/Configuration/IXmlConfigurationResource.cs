using System.Xml.Linq;

namespace CCLCC.Xrm.Sdk
{ 
    public interface IXmlConfigurationResource
    {
        XDocument Get(string key);

        XDocument Get(string key, bool disableCache);
    }
}
