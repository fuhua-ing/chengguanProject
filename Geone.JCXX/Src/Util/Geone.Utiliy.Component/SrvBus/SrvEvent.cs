using Newtonsoft.Json.Linq;

namespace Geone.Utiliy.Component
{
    public class SrvEvent : ISrvEvent
    {
        //执行
        public bool Event(string SubSrvId, JObject reqvalue)
        {
            return true;
        }

        public bool Event(string[] SubSrvIds, JObject reqvalue)
        {
            return true;
        }
    }
}