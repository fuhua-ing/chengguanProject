using MagicOnion;

namespace Geone.Utiliy.Component
{
    public interface IHealth : IService<IHealth>
    {
        UnaryResult<bool> Health();
    }
}