using Newtonsoft.Json.Linq;

namespace Geone.Utiliy.Component
{
    public interface ISrvEvent
    {
        bool Event(string SubSrvId, JObject reqvalue);

        bool Event(string[] SubSrvIds, JObject reqvalue);
    }
}