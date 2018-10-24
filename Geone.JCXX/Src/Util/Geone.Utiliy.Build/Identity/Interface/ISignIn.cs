namespace Geone.Utiliy.Build
{
    public interface ISignIn
    {
        bool Check(SignInIdentity identity, out string msg, out double? code, out string user, out string role, out int hour);
    }
}