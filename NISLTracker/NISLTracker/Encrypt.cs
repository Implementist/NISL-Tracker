using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NISLTracker
{
    abstract class Encrypt
    {
        /// <summary>
        /// 加密授权码明文获取密文
        /// </summary>
        /// <param name="plainText">授权码明文</param>
        /// <param name="securityStamp">安全戳（即盐值）</param>
        /// <returns>授权码加盐后取得的散列值</returns>
        public static string GetCiphertext(string plainText, string securityStamp)
        {
            byte[] mixture = Encoding.Default.GetBytes(plainText + securityStamp);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] ciphertext = md5.ComputeHash(mixture);
            return BitConverter.ToString(ciphertext).Replace("-", "");
        }

        /// <summary>
        /// 获取一个随机的安全戳
        /// </summary>
        /// <returns></returns>
        public static string GetSecurityStamp()
        {
            StringBuilder securityStamp = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                //每次取新生成的唯一识别码的首位
                securityStamp.Append(Guid.NewGuid().ToString().ToUpper()[0]);
            }
            return securityStamp.ToString();
        }
    }
}
