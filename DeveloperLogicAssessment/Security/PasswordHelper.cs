using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DeveloperLogicAssessment.Security
{
    public class PasswordHelper
    {
        private static string salt;

        public static void setSalt(string salt)
        {
            PasswordHelper.salt = salt;
        }

        private static string getSalt()
        {
            if (salt == null)
            {
                return ConfigurationManager.AppSettings["salt"];
            }

            return salt;
        }

        public static string Hash(string value)
        {
            return Hash(value, getSalt());
        }

        public static string Hash(string value, string salt)
        {
            return Hash(Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(salt));
        }

        public static string Hash(byte[] value, byte[] salt)
        {
            byte[] saltedValue = value.Concat(salt).ToArray();

            return Encoding.Default.GetString(new SHA256Managed().ComputeHash(saltedValue));
        }

        public static bool ConfirmPassword(string password, string hash)
        {
            string passwordHash = Hash(password);

            return passwordHash.SequenceEqual(hash);
        }
    }
}