using System;
using System.Collections.Generic;
using System.Linq;
using BankBalance.Accounts;
using BankBalance.Currency;
using WatiN.Core;

namespace BankBalance.BankInterfaces.Santander
{
    public class SantanderInterface : BankInterface<SantanderConfig>
    {
        private IList<Account> _accounts;
        public override IEnumerable<Account> Accounts { get { return _accounts; } }

        public DateTime _lastUpdated;
        public override DateTime LastUpdated { get { return _lastUpdated; } }

        public SantanderInterface(SantanderConfig config) : base(config)
        {
        }

        public override void LoadAccounts()
        {
            using (var browser = new IE("https://www.santander.co.uk"))
            {
                LogIn(browser);

                var accountList = browser.List(Find.ByClass("accountlist"));

                var accounts = new List<Account>();

                foreach (var accountListItem in accountList.ListItems)
                {
                    var accountName = accountListItem.Span(Find.ByClass("name")).Text;
                    var accountNumbers = accountListItem.Span(Find.ByClass("number")).Text;
                    var accountNumbersSplit = accountNumbers.Split(' ');
                    var accountNumber = accountNumbersSplit[1];
                    var sortCode = accountNumbersSplit[0];

                    var currentBalance = accountListItem.Span(Find.ByClass("amount")).Text;
                    var availableBalance = accountListItem.Span(Find.ByClass("extrainfo")).Text.Replace("Available:", "");

                    accounts.Add(new Account
                    {
                        AccountNumber = accountNumber,
                        AvailableBalance = new CurrencyValue(availableBalance),
                        Balance = new CurrencyValue(currentBalance),
                        Bank = Config.BankName,
                        Name = accountName,
                        SortCode = sortCode,
                        LastUpdated = DateTime.UtcNow
                    });
                }
                _accounts = accounts;
                _lastUpdated = DateTime.UtcNow;
            }
        }

        private void LogIn(IElementContainer browser)
        {
            browser.Link(Find.ByName("LoginLink")).Click();
            browser.TextField(Find.ById("infoLDAP_E.customerID")).Value = Config.Username;
            browser.Form("formCustomerID_1").Submit();

            if (browser.Elements.Count(el => el.Text == "We are unfamiliar with the computer you are using") != 0)
            {
                // Security Question
                var question = browser.Element(Find.ByText("Question:")).NextSibling.Text;
                question = question.TrimStart(' ').TrimEnd(' ');
                browser.TextField("cbQuestionChallenge.responseUser").Value = Config.SecurityAnswers[question];
                browser.Element(Find.ByValue("Continue")).Click();
            }

            browser.TextField("authentication.PassCode").Value = Config.Passcode;
            browser.TextField("authentication.ERN").Value = Config.RegistrationNumber;
            browser.Form(Find.ByName("hiddenForm")).Submit();
        }
    }
}
