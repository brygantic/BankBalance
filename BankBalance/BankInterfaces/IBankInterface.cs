using System.Collections.Generic;
using BankBalance.Accounts;

namespace BankBalance.BankInterfaces
{
    public interface IBankInterface
    {
        IEnumerable<Account> Accounts { get; }
        Account GetAccount(string accountNumber);
        void LoadAccounts();
    }
}
