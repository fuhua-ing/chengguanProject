using Newtonsoft.Json.Linq;

namespace Geone.Utiliy.Component
{
    public interface IRunningBlocking
    {
        bool Run(string queue, JObject reqvalue);
    }
}