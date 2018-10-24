using Newtonsoft.Json.Linq;

namespace Geone.Utiliy.Component
{
    public interface IRunningPubSub
    {
        bool Run(string channel, JObject reqvalue);
    }
}