using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BankBalance.Accounts;
using BankBalance.Currency;
using WatiN.Core;

namespace BankBalance.BankInterfaces.LloydsGroup.Tsb
{
    public class TsbInterface : BankInterface<TsbConfig>
    {
        private IList<Account> _accounts;

        public override IEnumerable<Account> Accounts { get { return _accounts; } }

        public TsbInterface(TsbConfig config) : base(config)
        {
        }

        public override void LoadAccounts()
        {
            using (var browser = new IE("http://www.tsb.co.uk/"))
            {
                LogIn(browser);

                var accountsList = browser.List(Find.ById("lstAccLst"));

                var accounts = new List<Account>();

                foreach (var accountListItem in accountsList.ListItems.Where(li => li.ClassName == "clearfix"))
                {
                    var accountDetailsText = accountListItem.Text;
                    var accountDetailsRegex = @"\s*(?<accountName>[\w ]*)\s*Sort Code(?<sortCode>\d\d-\d\d-\d\d), Account Number (?<accountNumber>\d{8})\s*Balance (?<balance>£\d*\.\d\d)";

                    var matches = Regex.Match(accountDetailsText, accountDetailsRegex).Groups;

                    accounts.Add(new Account
                    {
                        AccountNumber = matches["accountNumber"].Value,
                        AvailableBalance = null,
                        Balance = new CurrencyValue(matches["balance"].Value),
                        Bank = Config.BankName,
                        Name = matches["accountName"].Value,
                        SortCode = matches["sortCode"].Value
                    });
                }
                _accounts = accounts;
            }
        }

        private void LogIn(IE browser)
        {
            if (browser.Images.Count(image => image.Title == "Log back into your account") > 0)
            {
                // Just been logged out. Need to press the above image
                browser.Image(Find.ByTitle("Log back into your account")).Click();
            }
            else
            {
                browser.Link(Find.ByTitle("Log in to Internet Banking")).Click();
            }

            if (browser.Images.Count(image => image.Title == "Log back into your account") > 0)
            {
                // Just been logged out. Need to press the above image
                browser.Image(Find.ByTitle("Log back into your account")).Click();
            }

            browser.TextField(Find.ById("frmLogin:strCustomerLogin_userID")).Value = Config.UserId;
            browser.TextField(Find.ById("frmLogin:strCustomerLogin_pwd")).Value = Config.Password;
            browser.Form(Find.ByName("frmLogin")).Submit();

            LloydsGroupHelpers.FillLloydsGroupMemorableInformation(browser, Config);
            browser.Element(Find.ByName("frmentermemorableinformation1:btnContinue")).Click();
        }
    }
}
