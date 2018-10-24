namespace Geone.Utiliy.Build
{
    public class GeneralRegister : IRegister
    {
        public bool Check(SignInIdentity identity, out string msg, out double? code)
        {
            msg = null;
            code = null;
            return true;
        }
    }
}