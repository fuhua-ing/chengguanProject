using Geone.Utiliy.Component;
using MagicOnion;
using MagicOnion.Server;

namespace Geone.Utiliy.Build
{
    public class HealthRpc : ServiceBase<IHealth>, IHealth
    {
        public UnaryResult<bool> Health()
        {
            return UnaryResult(true);
        }
    }
}