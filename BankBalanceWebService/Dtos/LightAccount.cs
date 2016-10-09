using System;
using BankBalance.Accounts;

namespace BankBalanceWebService.Dtos
{
    public class LightAccount
    {
        public string Bank { get; }
        public string Name { get; }
        public string Balance { get; }
        public DateTime LastUpdated { get; }

        public LightAccount(Account account)
        {
            Bank = account.Bank;
            Name = account.Name;
            Balance = account.Balance.ToString();
            LastUpdated = account.LastUpdated;
        }
    }
}
