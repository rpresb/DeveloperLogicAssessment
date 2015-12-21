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
        public static string Hash(string value)
        {
            return Hash(value, ConfigurationManager.AppSettings["salt"]);
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