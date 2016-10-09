using System;
using System.Collections.Generic;
using System.Linq;
using BankBalance.Accounts;

namespace BankBalance.BankInterfaces
{
    public abstract class BankInterface<T> : IBankInterface where T : IBankInterfaceConfig
    {
        protected T Config;

        public abstract IEnumerable<Account> Accounts { get; }

        public Account GetAccount(string accountNumber)
        {
            return Accounts.First(account => account.AccountNumber == accountNumber);
        }

        public abstract DateTime LastUpdated { get; }
        
        public abstract void LoadAccounts();

        protected BankInterface(T config)
        {
            Config = config;
        }
    }
}
