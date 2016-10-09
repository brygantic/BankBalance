using System;
using BankBalance.Config;

namespace BankBalance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(SettingsManager.Get("Santander", "SecurityAnswers"));
            Console.ReadLine();
        }
    }
}
