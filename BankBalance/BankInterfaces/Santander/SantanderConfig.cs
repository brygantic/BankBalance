using System.Collections.Generic;

namespace BankBalance.BankInterfaces.Santander
{
    public class SantanderConfig : IBankInterfaceConfig
    {
        public string Username { get; set; }
        public IDictionary<string, string> SecurityAnswers { get; set; }
        public string Passcode { get; set; }
        public string RegistrationNumber { get; set; }

        public IDictionary<string, FieldType> RequiredFields
        {
            get
            {
                return new Dictionary<string, FieldType>
                {
                    {"Username", FieldType.String},
                    {"Passcode", FieldType.String},
                    {"RegistrationNumber", FieldType.String},
                    {"SecurityAnswers", FieldType.StringToStringMap}
                };
            }
        }
        public string BankName { get { return "Santander"; } }

        public bool IsPopulated
        {
            get
            {
                return Username != null &&
                       SecurityAnswers != null &&
                       Passcode != null &&
                       RegistrationNumber != null;
            }
        }
    }
}
