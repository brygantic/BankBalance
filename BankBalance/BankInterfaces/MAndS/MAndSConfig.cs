using System.Collections.Generic;

namespace BankBalance.BankInterfaces.MAndS
{
    public class MAndSConfig : IBankInterfaceConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecretAnswer { get; set; }

        public bool IsPopulated
        {
            get
            {
                return Username != null &&
                       Password != null &&
                       SecretAnswer != null;
            }
        }

        public IDictionary<string, FieldType> RequiredFields
        {
            get { return new Dictionary<string, FieldType>
            {
                {"Username", FieldType.String},
                {"Password", FieldType.String},
                {"SecretAnswer", FieldType.String}
            }; }
        }
        public string BankName { get { return "MAndS"; } }
    }
}
