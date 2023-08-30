using System;
using System.Security.Cryptography;
using System.Text;

namespace Common.Commons
{
    public static class HashHandle
    {
        public static string HashStringSHA256(string text, string secret_key)
        {
            var utf8 = new UTF8Encoding();
            var stringToSign_Byte = utf8.GetBytes(text);
            var keySecretByte = Encoding.UTF8.GetBytes(secret_key);
            var hashEngine = new HMACSHA256(keySecretByte);
            var hash = hashEngine.ComputeHash(stringToSign_Byte);
            var signature = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return signature;
        }

        public static string Encrypt(string data)
        {
            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(data);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string Decrypt(string data)
        {
            return ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(data));
        }

        //public static string CompareNewAndOldPassword(string newPassword, string oldPassword)
        //{
        //    // Case without change field password, type the original unhash password
        //    if (newPassword == oldPassword || Encrypt(newPassword) == oldPassword)
        //        return oldPassword;
        //    else
        //        return Encrypt(newPassword);
        //}
    }
}
