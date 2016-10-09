using System.Collections.Generic;

namespace BankBalance.BankInterfaces
{
    public interface IBankInterfaceConfig
    {
        IDictionary<string, FieldType> RequiredFields { get; }
        bool IsPopulated { get; }
        string BankName { get; }
    }

    public enum FieldType
    {
        String,
        StringToStringMap
    }
}
