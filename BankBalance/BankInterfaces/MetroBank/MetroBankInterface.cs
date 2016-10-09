using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using BankBalance.Accounts;
using BankBalance.BankInterfaces.LloydsGroup;
using BankBalance.Currency;
using WatiN.Core;

namespace BankBalance.BankInterfaces.MetroBank
{
    public class MetroBankInterface : BankInterface<MetroBankConfig>
    {
        private IList<Account> _accounts;
        public override IEnumerable<Account> Accounts { get { return _accounts; } }

        private DateTime _lastUpdated;
        public override DateTime LastUpdated { get { return _lastUpdated; } }


        public MetroBankInterface(MetroBankConfig config) : base(config)
        {
        }

        public override void LoadAccounts()
        {
            Settings.MakeNewIeInstanceVisible = false;
            using (var browser = new IE("https://personal.metrobankonline.co.uk/MetroBankRetail/"))
            {
                Login(browser);

                var accountsDiv = browser.Div(Find.ById("p1_GRP_0936EF1D3ED8C8E960870"));

                var accounts = new List<Account>();

                foreach (var accountDiv in accountsDiv.Divs.Where(div => div.ClassName == "account-container  "))
                {
                    var accountName = accountDiv.Links.First(link => link.Title != "AccountIcon").Title;
                    var sortCode = accountDiv.Label(Find.ByText("Sort Code")).NextSibling.NextSibling.Text;
                    var accountNumber = accountDiv.Div(Find.ByClass("leftMargin10px")).Spans.First().Text;

                    var account = new Account
                    {
                        Name = accountName,
                        SortCode = sortCode,
                        AccountNumber = accountNumber,
                        LastUpdated = DateTime.UtcNow
                    };

                    // Current Balance

                    var currentBalanceSpan =
                        accountDiv.Spans.First(
                            span => span.ClassName == "balance" || span.ClassName == "negative-balance");

                    string balance;
                    if (currentBalanceSpan.ClassName == "balance")
                    {
                        balance = currentBalanceSpan.Text + currentBalanceSpan.NextSibling.Text;
                    }
                    else
                    {
                        balance = currentBalanceSpan.Text;
                    }
                    account.Balance = new CurrencyValue(balance);

                    accounts.Add(account);
                }
                   
                _accounts = accounts;
                _lastUpdated = DateTime.UtcNow;
            }
        }

        private void Login(IE browser)
        {
            browser.WaitForComplete();
            Thread.Sleep(1000); // MetroBank tries to load from Cookies, overwriting if we're too quick
            browser.TextField(Find.ById("USER_NAME")).Value = Config.Username;

            browser.Button(Find.ByTitle("Continue")).Click();

            browser.TextField(Find.ById("LOGIN_PASSWORD")).Value = Config.Password;
            FillSecurityNumber(browser);
            browser.Button(Find.ByTitle("Log in")).Click();

            browser.Button(Find.ByTitle("View my accounts")).Click();
        }

        private void FillSecurityNumber(IE browser)
        {
            var securityDropdownsDiv = browser.Div(Find.ById("security-dropdowns"));
            var numbersRequestedAsStrings = securityDropdownsDiv.Paras.Select(p => p.Text);

            var numbersRequestedAsInts = numbersRequestedAsStrings.Select(str => int.Parse(Regex.Match(str, @"\d+").Value));
            numbersRequestedAsInts = numbersRequestedAsInts.OrderBy(i => i);

            var securityNumbersString = string.Join("", numbersRequestedAsInts.Select(i => Config.SecurityNumber[i - 1]));
            browser.Element(
                Find.ByName("METROBANK[1].LOGIN[1].SECURITYNUMBER[1].APPENDEDVALUE"))
                .SetAttributeValue("value", securityNumbersString);
        }
    }
}
