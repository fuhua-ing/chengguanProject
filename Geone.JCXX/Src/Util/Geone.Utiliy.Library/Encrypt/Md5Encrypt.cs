using System.Security.Cryptography;
using System.Text;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// MD5加密操作类
    /// </summary>
    public class Md5Encrypt
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="source">加密字符</param>
        /// <param name="code">加密位数16/32</param>
        /// <returns></returns>
        public static string Encrypt(string source, int code)
        {
            string strEncrypt = string.Empty;
            if (code == 16)
            {
                MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(source));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                strEncrypt = sBuilder.ToString().Substring(8, 16);
            }

            if (code == 32)
            {
                MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(source));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                strEncrypt = sBuilder.ToString();
            }

            return strEncrypt;
        }
    }
}