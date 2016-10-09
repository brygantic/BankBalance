using System;
using System.Collections.Generic;
using System.Configuration;
using BankBalance.BankInterfaces;

namespace BankBalance.Config
{
    public static class ConfigLoader
    {
        public static TConfig GetConfig<TConfig>() where TConfig : IBankInterfaceConfig, new()
        {
            var config = new TConfig();
            var requiredFields = config.RequiredFields;
            var bankName = config.BankName;

            var loadedConfig = GetValuesFromConfig(bankName, requiredFields);

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

            return config;
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
                        string.Format("Could not load {0} config without a {0}.{1} key in appSettings",
                            configCategory, fieldKey));
                }
            }
            return loadedConfig;
        }

        private static Dictionary<string, string> ParseSecurityAnswers(string securityAnswersString)
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
    }
}
