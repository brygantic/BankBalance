using System.Text.RegularExpressions;
using WatiN.Core;

namespace BankBalance.BankInterfaces.LloydsGroup
{
    public static class LloydsGroupHelpers
    {
        public static void FillLloydsGroupMemorableInformation(IElementContainer browser, LloydsGroupConfig config)
        {
            for (var inputNumber = 1; inputNumber <= 3; inputNumber++)
            {
                var id = string.Format("frmentermemorableinformation1:strEnterMemorableInformation_memInfo{0}",
                    inputNumber);
                var memInfoInput = browser.SelectList(Find.ById(id));

                var characterLabel = memInfoInput.PreviousSibling.Text;
                var characterNumber =
                    int.Parse(Regex.Match(characterLabel, @"Character (?<charNum>\d*)").Groups["charNum"].Value);

                var zeroIndexedCharacterNumber = characterNumber - 1;

                var value = string.Format("&nbsp;{0}", config.MemorableInformation[zeroIndexedCharacterNumber]);
                memInfoInput.SelectByValue(value);
            }
        }
    }
}
