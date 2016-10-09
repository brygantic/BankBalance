using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BankBalance.Accounts;
using BankBalance.Currency;
using WatiN.Core;

namespace BankBalance.BankInterfaces.MAndS
{
    public class MAndSInterface : BankInterface<MAndSConfig>
    {
        private IList<Account> _accounts;

        public override IEnumerable<Account> Accounts { get { return _accounts; } }

        public MAndSInterface(MAndSConfig config) : base(config)
        {
        }

        // 

        public override void LoadAccounts()
        {
            using (var browser = new IE("http://bank.marksandspencer.com/banking/my-account/"))
            {
                LogIn(browser);

                var accountsTable = browser.Table(Find.ByClass("csTable"));

                var accounts = new List<Account>();

                foreach (var tableBody in accountsTable.TableBodies)
                {
                    var accountName = tableBody.Link(Find.ByClass("csAct")).Children().First().Text;
                    var accountNumber = tableBody.TableCell(Find.ByClass("csFirst")).NextSibling.Text;

                    var currentBalance = tableBody.TableCell(Find.ByClass("csLast")).PreviousSibling.Text;
                    if (currentBalance[currentBalance.Length - 1] == 'D')
                    {
                        currentBalance = string.Format("-{0}", currentBalance.Substring(0, currentBalance.Length - 2));
                    }

                    accounts.Add(new Account
                    {
                        AccountNumber = accountNumber,
                        AvailableBalance = null,
                        Balance = new CurrencyValue(currentBalance),
                        Bank = "M&S",
                        Name = accountName,
                        SortCode = null
                    });
                }
                _accounts = accounts;
            }
        }

        private void LogIn(IE browser)
        {
            browser.TextField(Find.ById("userid1")).Value = Config.Username;
            browser.Button(Find.ByTitle("Sign in")).Click();

            FillInPasswordCharacters(browser);
            browser.TextField(Find.ById("memorableAnswer")).Value = Config.SecretAnswer;
            browser.Element(Find.ByTitle("Continue")).Click();
        }

        private void FillInPasswordCharacters(IE browser)
        {
            var partialPasswordInputDiv = browser.Div(Find.ByClass("csFields"));
            var partialPasswordInputText = partialPasswordInputDiv.Text;

            var regexMatcher =
                @"(?<first>\d+\w{2})\s*(\d+\w{2}) letter from your passwordis required\s*(?<second>\d+\w{2}|Next to Last)\s*(\d+\w{2}|Next to Last) letter from your passwordis required\s*(?<third>\d+\w{2}|Next to Last|Last)";

            var groups = Regex.Match(partialPasswordInputText, regexMatcher).Groups;
            
            var intToPositionMap = new Dictionary<int, string>
            {
                {0, "first"},
                {1, "second"},
                {2, "third"}
            };

            foreach (var map in intToPositionMap)
            {
                int charIndex;
                if (groups[map.Value].Value == "Last")
                {
                    charIndex = Config.Password.Length - 1;
                }
                else if (groups[map.Value].Value == "Next to Last")
                {
                    charIndex = Config.Password.Length - 2;
                }
                else
                {
                    // Get rid of nd/th/rd
                    var justNumber = groups[map.Value].Value.Substring(0, groups[map.Value].Value.Length - 2);
                    charIndex = int.Parse(justNumber) - 1; // 0-index it
                }

                var id = string.Format("keyrcc_password_{0}", map.Value);
                partialPasswordInputDiv.TextField(Find.ById(id)).Value = Config.Password[charIndex].ToString();
            }
        }
    }
}
