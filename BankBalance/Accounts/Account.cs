using BankBalance.Currency;

namespace BankBalance.Accounts
{
    public class Account
    {
        public string Bank { get; set; }
        public string Name { get; set; }
        public CurrencyValue Balance { get; set; }
        public CurrencyValue AvailableBalance { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }

        public override string ToString()
        {
            return
                string.Format(
                    "<Account>{{ {0}.{1}, AccountNumber: {2}, SortCode: {3}, Balance: {4}, AvailableBalance: {5} }}",
                    Bank, Name, AccountNumber, SortCode, Balance, AvailableBalance);
        }
    }
}
