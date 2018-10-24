namespace Geone.Utiliy.Build
{
    public interface IRegister
    {
        bool Check(SignInIdentity identity, out string msg, out double? code);
    }
}