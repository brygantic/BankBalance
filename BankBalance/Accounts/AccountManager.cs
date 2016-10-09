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

        private readonly object _interfacesToLoadLock = new object();
        private int _interfacesToLoad;

        public bool InitialLoadsComplete
        {
            get { return _interfacesToLoad == 0; }
        }

        public IEnumerable<Account> Accounts
        {
            get { return _bankInterfaces.SelectMany(bi => bi.Accounts); }
        }

        public Account GetAccount(string accountNumber)
        {
            return Accounts.First(account => account.AccountNumber == accountNumber);
        }

        public AccountManager(bool doShowIE = false)
        {
            WatiN.Core.Settings.MakeNewIeInstanceVisible = doShowIE;

            _bankInterfaces = new List<IBankInterface>();
            _interfacesToLoad = 0;
        }

        public bool TryAddInterface<TBankInterface, TConfig>(Func<TConfig, TBankInterface> interfaceCreator, TConfig config)
            where TBankInterface : BankInterface<TConfig>
            where TConfig : IBankInterfaceConfig
        {
            if (!config.IsPopulated)
            {
                return false;
            }
            var bankInterface = interfaceCreator(config);
            _bankInterfaces.Add(bankInterface);

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
            // Required for WatiN to use IE
            loadingThread.SetApartmentState(ApartmentState.STA);
            loadingThread.Start();

            return true;
        }

        public void Reload<TBankInterface>() where TBankInterface : IBankInterface
        {
            foreach (var bankInterface in _bankInterfaces.Where(bi => bi is TBankInterface))
            {
                Load(bankInterface);
            }
        }

        public void Reload()
        {
            foreach (var bankInterface in _bankInterfaces)
            {
                Load(bankInterface);
            }
        }

        private void Load(IBankInterface bankInterface)
        {
            var loadingThread = new Thread(bankInterface.LoadAccounts);
            // Required for WatiN to use IE
            loadingThread.SetApartmentState(ApartmentState.STA);
            loadingThread.Start();
        }
    }
}
