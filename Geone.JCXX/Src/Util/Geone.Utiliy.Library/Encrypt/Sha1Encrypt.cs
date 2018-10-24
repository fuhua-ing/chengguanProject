using System.Security.Cryptography;
using System.Text;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// SHA1加密操作类
    /// </summary>
    public class Sha1Encrypt
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Encrypt(string source)
        {
            byte[] StrRes = Encoding.Default.GetBytes(source);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }
    }
}