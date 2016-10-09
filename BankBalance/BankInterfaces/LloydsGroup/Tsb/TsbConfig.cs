namespace BankBalance.BankInterfaces.LloydsGroup.Tsb
{
    public class TsbConfig : LloydsGroupConfig, IBankInterfaceConfig
    {
        public string BankName { get { return "TSB"; } }
    }
}
