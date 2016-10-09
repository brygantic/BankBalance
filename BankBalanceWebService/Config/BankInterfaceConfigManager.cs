using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BankBalance.BankInterfaces;

namespace BankBalanceWebService.Config
{
    public static class BankInterfaceConfigManager
    {
        public static bool TryGetConfig<TConfig>(out TConfig config) where TConfig : IBankInterfaceConfig, new()
        {
            config = new TConfig();
            var requiredFields = config.RequiredFields;
            var bankName = config.BankName;

            Dictionary<string, string> loadedConfig;
            try
            {
                loadedConfig = GetValuesFromConfig(bankName, requiredFields);
            }
            catch (ConfigurationErrorsException)
            {
                return false;
            }

            foreach (var keyValuePair in loadedConfig)
            {
                if (requiredFields[keyValuePair.Key] == FieldType.StringToStringMap)
                {
                    config.GetType().GetProperty(keyValuePair.Key).SetValue(config, ParseSecurityAnswers(keyValuePair.Value));
                }
                else
                {
                    config.GetType().GetProperty(keyValuePair.Key).SetValue(config, keyValuePair.Value);
                }
            }

            return true;
        }

        public static void SetConfig<TConfig>(TConfig config) where TConfig : IBankInterfaceConfig
        {
            foreach (var keyValuePair in config.RequiredFields)
            {
                object objectToSave = config.GetType().GetProperty(keyValuePair.Key).GetValue(config);
                string stringToSave;
                if (keyValuePair.Value == FieldType.StringToStringMap)
                {
                    stringToSave = ParseSecurityAnswers((Dictionary<string, string>)objectToSave);
                }
                else
                {
                    stringToSave = objectToSave.ToString();
                }

                SettingsManager.Set(config.BankName, keyValuePair.Key, stringToSave);
            }
        }

        private static Dictionary<string, string> GetValuesFromConfig(string configCategory, IDictionary<string, FieldType> fieldKeys)
        {
            Dictionary<string, string> loadedConfig = new Dictionary<string, string>();
            foreach (var fieldKey in fieldKeys.Keys)
            {
                try
                {
                    loadedConfig[fieldKey] = SettingsManager.Get(configCategory, fieldKey);
                }
                catch (ConfigurationErrorsException)
                {
                    throw new ConfigurationErrorsException(
                        string.Format("Could not load {0} config without a {0}{1} key in appSettings",
                            configCategory, fieldKey));
                }
            }
            return loadedConfig;
        }

        public static Dictionary<string, string> ParseSecurityAnswers(string securityAnswersString)
        {
            var returnDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            var securityAnswerStrings = securityAnswersString.TrimEnd(';').Split(';');

            foreach (var securityAnswerString in securityAnswerStrings)
            {
                var split = securityAnswerString.Split('=');

                if (split.Length != 2)
                {
                    throw new ConfigurationErrorsException(
                        "Could not parse string \"{0}\" into a security answer. Expected \"question=answer\"");
                }

                returnDict[split[0]] = split[1];
            }

            return returnDict;
        }

        public static string ParseSecurityAnswers(Dictionary<string, string> securityAnswers)
        {
            return string.Join(";", securityAnswers.Select(sa => sa.Key + "=" + sa.Value));
        }
    }
}
