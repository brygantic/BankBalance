using System.Collections.Generic;

namespace BankBalance.BankInterfaces.MetroBank
{
    public class MetroBankConfig : IBankInterfaceConfig
    {
        public string Username { get; set; }
        public string SecurityNumber { get;  set; }
        public string Password { get; set; }

        public IDictionary<string, FieldType> RequiredFields
        {
            get
            {
                return new Dictionary<string, FieldType>
                {
                    {"Username", FieldType.String},
                    {"SecurityNumber", FieldType.String},
                    {"Password", FieldType.String}
                };
            }
        }

        public bool IsPopulated
        {
            get
            {
                return Username != null &&
                       SecurityNumber != null &&
                       Password != null;
            }
        }

        public string BankName { get { return "MetroBank"; } }
    }
}

