using Newtonsoft.Json.Linq;
using System.Threading;

namespace Geone.Utiliy.Component
{
    public interface IRunningTimer
    {
        Timer runningtimer { get; set; }

        bool Run(JObject reqvalue);
    }
}