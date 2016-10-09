using System.Collections.Generic;
using System.Linq;
using BankBalance.Accounts;
using BankBalance.Currency;
using WatiN.Core;

namespace BankBalance.BankInterfaces.LloydsGroup.Halifax
{
    public class HalifaxInterface : BankInterface<HalifaxConfig>
    {
        private IList<Account> _accounts;

        public override IEnumerable<Account> Accounts { get { return _accounts; } }

        public HalifaxInterface(HalifaxConfig config) : base(config)
        {
        }

        public override void LoadAccounts()
        {
            using (var browser = new IE("http://www.halifax.co.uk/"))
            {
                LogIn(browser);

                var accounts = new List<Account>();

                var accountDivs = browser.Divs.Where(div => div.ClassName == "des-m-sat-xx-account-information");

                var accountDivNumber = 1;
                foreach (var accountDiv in accountDivs)
                {
                    var accountName =
                        accountDiv.Link(Find.ById(string.Format("lnkAccName_des-m-sat-xx-{0}", accountDivNumber))).Text;

                    var accountNumber = accountDiv.Element(Find.ByText("Account Number")).NextSibling.Text;
                    var sortCode = accountDiv.Element(Find.ByText("Sort code")).NextSibling.Text;

                    var currentBalance = accountDiv.Spans.First(span => span.Text.Contains("£")).Text;

                    accounts.Add(new Account
                    {
                        AccountNumber = accountNumber,
                        AvailableBalance = null,
                        Balance = new CurrencyValue(currentBalance),
                        Bank = Config.BankName,
                        Name = accountName,
                        SortCode = sortCode
                    });

                    accountDivNumber++;
                }                
                _accounts = accounts;
            }
        }

        private void LogIn(IE browser)
        {
            if (browser.Links.Count(link => link.Title == "Log back into your account") > 0)
            {
                // Just been logged out. Need to press the above link
                browser.Link(Find.ByTitle("Log back into your account")).Click();
            }
            else
            {
                browser.Link(Find.ByTitle("Sign in ")).Click();
            }

            if (browser.Links.Count(link => link.Title == "Log back into your account") > 0)
            {
                // Just been logged out. Need to press the above link
                browser.Link(Find.ByTitle("Log back into your account")).Click();
            }

            browser.TextField(Find.ById("frmLogin:strCustomerLogin_userID")).Value = Config.UserId;
            browser.TextField(Find.ById("frmLogin:strCustomerLogin_pwd")).Value = Config.Password;
            browser.Form(Find.ByName("frmLogin")).Submit();

            LloydsGroupHelpers.FillLloydsGroupMemorableInformation(browser, Config);
            browser.Element(Find.ByName("frmentermemorableinformation1:btnContinue")).Click();
        }
    }
}
