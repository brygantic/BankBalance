using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BankBalance.BankInterfaces;
using BankBalance.Config;

namespace BankBalance.Accounts
{
    public class AccountManager
    {
        private readonly IList<IBankInterface> _bankInterfaces;

        private object _interfacesToLoadLock = new object();
        private int _interfacesToLoad;
        public bool InitialLoadsComplete { get { return _interfacesToLoad == 0; } }

        public IEnumerable<Account> Accounts
        {
            get { return _bankInterfaces.SelectMany(bi => bi.Accounts); }
        }

        public AccountManager()
        {
            _bankInterfaces = new List<IBankInterface>();
            _interfacesToLoad = 0;
        }

        public bool TryAddInterface<TBankInterface, TConfig>(Func<TConfig, TBankInterface> interfaceCreator)
            where TBankInterface : BankInterface<TConfig>
            where TConfig : IBankInterfaceConfig, new()
        {
            var config = ConfigLoader.GetConfig<TConfig>();
            if (!config.IsPopulated)
            {
                if (!GetConfigFromUser(out config))
                {
                    // Can't use this interface
                    return false;
                }
            }
            var bankInterface = interfaceCreator(config);
            _bankInterfaces.Add(bankInterface);

            // TODO: Change this to be done in a separate thread
            lock (_interfacesToLoadLock)
            {
                _interfacesToLoad++;
            }

            var loadingThread = new Thread(() =>
            {
                bankInterface.LoadAccounts();
                lock (_interfacesToLoadLock)
                {
                    _interfacesToLoad--;
                }
            });
            loadingThread.SetApartmentState(ApartmentState.STA);
            loadingThread.Start();
            return true;
        }

        private bool GetConfigFromUser<TConfig>(out TConfig config) where TConfig : IBankInterfaceConfig, new()
        {
            config = new TConfig();
            if (ReadLineYesNo(string.Format("Would you like to configure a {0} online account?", config.BankName)))
            {
                GetValuesFromUserAndSave(config.RequiredFields, config.BankName);
                config = ConfigLoader.GetConfig<TConfig>();
                return true;
            }
            return false;
        }

        private static void GetValuesFromUserAndSave(IDictionary<string, FieldType> requiredFields, string configCategory)
        {
            foreach (var kvp in requiredFields)
            {
                Console.Write("{0}: ", kvp.Key);

                if (kvp.Value == FieldType.StringToStringMap)
                {
                    Console.Write("(Enter in the format \"key=value;key2=value2\")");
                }

                var fieldValue = Console.ReadLine();
                SettingsManager.Set(configCategory, kvp.Key, fieldValue);
            }
        }

        private static bool ReadLineYesNo(string message)
        {
            return ReadLineOptions(message, new[] {"y", "n"}) == "y";
        }

        private static string ReadLineOptions(string message, IList<string> options)
        {
            var result = "";
            var first = true;
            while (!options.Contains(result, StringComparer.OrdinalIgnoreCase))
            {
                if (!first)
                {
                    Console.WriteLine("Invalid input");
                }
                first = false;
                Console.WriteLine("{0}\n[{1}]", message, string.Join(", ", options));
                result = Console.ReadLine();
            }
            return options.First(option => option.Equals(result, StringComparison.OrdinalIgnoreCase));
        }
    }
}
