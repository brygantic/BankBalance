using System;
using System.Security.Cryptography;
using System.Text;

namespace BankBalance.Config
{
    public static class SettingsManager
    {
        private static string Crypt(string text)
        {
            return Convert.ToBase64String(
                ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(text), null, DataProtectionScope.CurrentUser));
        }

        private static string Decrypt(string text)
        {
            return Encoding.Unicode.GetString(
                ProtectedData.Unprotect(
                     Convert.FromBase64String(text), null, DataProtectionScope.CurrentUser));
        }

        public static void Set(string category, string key, string value)
        {
            Properties.Settings.Default[GetSettingsKey(category, key)] = Crypt(value);
            Properties.Settings.Default.Save();
        }

        public static string Get(string category, string key)
        {
            var setting = Properties.Settings.Default[GetSettingsKey(category, key)];
            try
            {
                return Decrypt(setting.ToString());
            }
            catch
            {
                return null;
            }
        }

        private static string GetSettingsKey(string category, string key)
        {
            return string.Format("{0}{1}", category, key);
        }
    }
}
