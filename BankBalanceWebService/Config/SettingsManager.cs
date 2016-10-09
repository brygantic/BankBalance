using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;

namespace BankBalanceWebService.Config
{
    public static class SettingsManager
    {
        public static void Set(string category, string key, string value)
        {
            throw new NotImplementedException();
            WebConfigurationManager.AppSettings[GetSettingsKey(category, key)] = Crypt(value);
        }

        public static string Get(string category, string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[GetSettingsKey(category, key)];
            var setting = WebConfigurationManager.AppSettings[GetSettingsKey(category, key)];
            try
            {
                return Decrypt(setting.ToString());
            }
            catch
            {
                return null;
            }
        }

        private static string Crypt(string text)
        {
            return Convert.ToBase64String(
                ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(text), null, DataProtectionScope.LocalMachine));
        }

        private static string Decrypt(string text)
        {
            return Encoding.Unicode.GetString(
                ProtectedData.Unprotect(
                     Convert.FromBase64String(text), null, DataProtectionScope.LocalMachine));
        }

        private static string GetSettingsKey(string category, string key)
        {
            return string.Format("{0}{1}", category, key);
        }
    }
}
