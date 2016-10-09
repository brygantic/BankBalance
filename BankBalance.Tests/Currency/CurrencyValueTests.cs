using BankBalance.Currency;
using NFluent;
using NUnit.Framework;

namespace BankBalance.Test.Currency
{
    [TestFixture]
    public class CurrencyValueTests
    {
        [Test]
        public void Test_PositiveBelowThousandInput_ToString_AsExpected()
        {
            Check.That(new CurrencyValue("£123.45").ToString()).Equals("£123.45");
        }

        [Test]
        public void Test_PositiveAboveThousandInput_WithSeparator_To_String_AsExpected()
        {
            Check.That(new CurrencyValue("£12,345.67").ToString()).Equals("£12,345.67");
        }

        [Test]
        public void Test_PositiveAboveThousandInput_WithoutSeparator_To_String_AsExpected()
        {
            Check.That(new CurrencyValue("£12345.67").ToString()).Equals("£12,345.67");
        }

        [Test]
        public void Test_NegativeBelowThousandInput_MinusBeforeSymbol_ToString_AsExpected()
        {
            Check.That(new CurrencyValue("-£12.34").ToString()).Equals("-£12.34");
        }

        [Test]
        public void Test_NegativeBelowThousandInput_MinusAfterSymbol_ToString_AsExpected()
        {
            Check.That(new CurrencyValue("£-12.34").ToString()).Equals("-£12.34");
        }

        [Test]
        public void Test_NegativeAboveThousandInput_ToString_AsExpected()
        {
            Check.That(new CurrencyValue("-£12345.67").ToString()).Equals("-£12,345.67");
        }

        [Test]
        public void Test_WhitespaceAroundValue_ToString_AsExpected()
        {
            Check.That(new CurrencyValue("  £ 12.34").ToString()).Equals("£12.34");
        }

        [Test]
        public void Test_NegativeIndicatedWithD_ToString_AsExpected()
        {
            Check.That(new CurrencyValue("£ 1234.56 D").ToString()).Equals("-£1,234.56");
        }

        [Test]
        public void Test_SymbolAtEnd_ToString_AsExpected()
        {
            Check.That(new CurrencyValue("12345.67£").ToString()).Equals("£12,345.67");
        }

        [Test]
        public void Test_CentsZero_ToString_CentsArePadded()
        {
            Check.That(new CurrencyValue("£1234.0").ToString()).Equals("£1,234.00");
        }

        [Test]
        public void Test_CentsFive_ToString_CentsArePadded()
        {
            Check.That(new CurrencyValue("£1234.05").ToString()).Equals("£1,234.05");
        }

        [Test]
        public void Test_NoCentsGiven_DoesNotBlowUp()
        {
            Check.That(new CurrencyValue("£12345").ToString()).Equals("£12,345.00");
        }

        [Test]
        public void Test_TwoEqualValues_AreEqual()
        {
            var value1 = new CurrencyValue("£123.45");
            var value2 = new CurrencyValue("123.45 £");

            Check.That(value1.Equals(value2)).IsTrue();
        }

        [Test]
        public void Test_TwoEqualValues_DifferentSymbols_AreNotEqual()
        {
            var value1 = new CurrencyValue("£123.45");
            var value2 = new CurrencyValue("$123.45");

            Check.That(value1.Equals(value2)).IsFalse();
        }
    }
}
