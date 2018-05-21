using System.Xml.Linq;

namespace CCLLC.Xrm.Sdk
{ 
    public interface IXmlConfigurationResource
    {
        XDocument Get(string key);

        XDocument Get(string key, bool disableCache);
    }
}
