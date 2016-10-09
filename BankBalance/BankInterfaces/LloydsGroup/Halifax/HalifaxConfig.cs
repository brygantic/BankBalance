namespace BankBalance.BankInterfaces.LloydsGroup.Halifax
{
    public class HalifaxConfig : LloydsGroupConfig, IBankInterfaceConfig
    {
        public string BankName { get { return "Halifax"; } }
    }
}
