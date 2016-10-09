using System.Collections.Generic;

namespace BankBalance.BankInterfaces.LloydsGroup
{
    public abstract class LloydsGroupConfig
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string MemorableInformation { get; set; }

        public bool IsPopulated
        {
            get
            {
                return UserId != null &&
                       Password != null &&
                       MemorableInformation != null;
            }
        }

        public IDictionary<string, FieldType> RequiredFields
        {
            get
            {
                return new Dictionary<string, FieldType>
                {
                    {"UserId", FieldType.String},
                    {"Password", FieldType.String},
                    {"MemorableInformation", FieldType.String}
                };
            }
        }
    }
}
