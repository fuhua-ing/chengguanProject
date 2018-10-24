using System;
using System.Security.Cryptography;
using System.Text;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// Des加解密操作类
    /// </summary>
    public class DesEncrypt
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Encrypt(string source)
        {
            return Encrypt(source, "geone###***");
        }

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Encrypt(string source, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(source);

            string pmd = Md5Encrypt.Encrypt(sKey, 32).ToUpper();

            des.Key = Encoding.ASCII.GetBytes(pmd.Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(pmd.Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Decrypt(string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                return Decrypt(source, "geone###***");
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt(string source, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = source.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, a;
            for (x = 0; x < len; x++)
            {
                a = Convert.ToInt32(source.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)a;
            }

            string pmd = Md5Encrypt.Encrypt(sKey, 32).ToUpper();

            des.Key = Encoding.ASCII.GetBytes(pmd.Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(pmd.Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }
    }
}