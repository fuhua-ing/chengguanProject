using Geone.Utiliy.Library;
using MagicOnion;

namespace Geone.Utiliy.Component
{
    public interface IBuild : IService<IBuild>
    {
        UnaryResult<AppInitialize> ServiceRegister(AppSetting setting);

        UnaryResult<AppInitialize> ServiceObtainById(string Id);

        UnaryResult<AppInitialize> ServiceObtainByName(string Name);
    }
}