using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BankBalance.Accounts;
using BankBalance.BankInterfaces.LloydsGroup.Halifax;
using BankBalance.BankInterfaces.LloydsGroup.Tsb;
using BankBalance.BankInterfaces.MAndS;
using BankBalance.BankInterfaces.MetroBank;
using BankBalance.BankInterfaces.Santander;

namespace BankBalance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WatiN.Core.Settings.MakeNewIeInstanceVisible = false;

            var manager = new AccountManager();

            manager.TryAddInterface<MetroBankInterface, MetroBankConfig>(config => new MetroBankInterface(config));
             
            manager.TryAddInterface<MAndSInterface, MAndSConfig>(config => new MAndSInterface(config));
            manager.TryAddInterface<TsbInterface, TsbConfig>(config => new TsbInterface(config));
            manager.TryAddInterface<SantanderInterface, SantanderConfig>(config => new SantanderInterface(config));
            manager.TryAddInterface<HalifaxInterface, HalifaxConfig>(config => new HalifaxInterface(config));

            while (!manager.InitialLoadsComplete)
            {
                Thread.Sleep(2500);
            }

            foreach (var account in manager.Accounts)
            {
                Console.WriteLine(account);
            }

            Console.ReadLine();
        }
    }
}
