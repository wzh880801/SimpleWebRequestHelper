using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace SimpleWebRequestHelper.Helper
{
    public static class CryptHelper
    {
        public static string ComputeMd5(byte[] bytes)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] retVal = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}