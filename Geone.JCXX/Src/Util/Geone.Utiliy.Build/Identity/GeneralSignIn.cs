namespace Geone.Utiliy.Build
{
    public class GeneralSignIn : ISignIn
    {
        public bool Check(SignInIdentity identity, out string msg, out double? code, out string user, out string role, out int hour)
        {
            hour = 6;
            msg = null;
            code = null;
            user = string.Empty;
            role = string.Empty;
            return true;
        }
    }
}