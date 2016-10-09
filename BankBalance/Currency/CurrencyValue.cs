using System.Text.RegularExpressions;

namespace BankBalance.Currency
{
    public class CurrencyValue
    {
        private bool IsNegative { get; set; }

        private uint AbsoluteUnits { get; set; }
        private uint AbsoluteCents { get; set; }

        private string PaddedCents
        {
            get
            {
                if (AbsoluteCents < 10)
                {
                    return "0" + AbsoluteCents;
                }
                return AbsoluteCents.ToString();
            }
        }

        public string Symbol { get; set; }

        private readonly string _thousandsSeparator = ",";
        private readonly string _decimalSeparator = ".";

        public override string ToString()
        {
            var returnString = IsNegative ? "-" : "";
            returnString = returnString + Symbol;
            returnString = returnString + ApplyThousandsSeparator(AbsoluteUnits);
            returnString = returnString + _decimalSeparator + PaddedCents;

            return returnString;
        }

        public CurrencyValue(string currencyString)
        {
            currencyString = Regex.Replace(currencyString, @"\s*", "");
            currencyString = Regex.Replace(currencyString, _thousandsSeparator, "");
            currencyString = ExtractSymbol(currencyString);
            currencyString = ExtractNegativity(currencyString);
            ExtractAbsoluteValues(currencyString);
        }

        private string ExtractSymbol(string currencyString)
        {
            foreach (var potentialSymbol in new[] { "£", "$", "€" })
            {
                if (currencyString.Contains(potentialSymbol))
                {
                    Symbol = potentialSymbol;
                    return currencyString.Replace(potentialSymbol, "");
                }
            }
            Symbol = "?";
            return currencyString;
        }

        private string ExtractNegativity(string currencyString)
        {
            if (currencyString.Contains("-"))
            {
                IsNegative = true;
                return currencyString.Replace("-", "");
            }

            if (currencyString.Contains("D"))
            {
                IsNegative = true;
                return currencyString.Replace("D", "");
            }

            if (currencyString.Contains("C"))
            {
                IsNegative = false;
                return currencyString.Replace("C", "");
            }

            IsNegative = false;
            return currencyString;
        }

        private void ExtractAbsoluteValues(string currencyString)
        {
            var split = currencyString.Split(_decimalSeparator.ToCharArray()[0]);
            AbsoluteUnits = uint.Parse(split[0]);
            if (split.Length > 1)
            {
                AbsoluteCents = uint.Parse(split[1]);
            }
            else
            {
                AbsoluteCents = 0;
            }
        }

        private string ApplyThousandsSeparator(uint value)
        {
            var originalValueString = value.ToString();

            var returnString = "";
            int i;
            for (i = originalValueString.Length - 3; i > 0; i -= 3)
            {
                returnString += _thousandsSeparator + originalValueString.Substring(i, 3);
            }

            returnString = originalValueString.Substring(0, 3 + i) + returnString;

            return returnString;
        }

        public override bool Equals(object other)
        {
            if (other is float)
            {
                return Equals((float)other);
            }
            if (other is CurrencyValue)
            {
                return Equals((CurrencyValue)other);
            }
            return false;
        }

        public bool Equals(float other)
        {
            var thisAsFloat = float.Parse(string.Format("{0}.{1}", AbsoluteUnits, AbsoluteCents));
            if (IsNegative)
            {
                thisAsFloat = -thisAsFloat;
            }
            return thisAsFloat.Equals(other);
        }

        public bool Equals(CurrencyValue other)
        {
            return ToString() == other.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
