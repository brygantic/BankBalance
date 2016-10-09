using System;
using System.Threading;
using BankBalance.Accounts;
using BankBalance.BankInterfaces.LloydsGroup.Halifax;
using BankBalance.BankInterfaces.LloydsGroup.Tsb;
using BankBalance.BankInterfaces.MAndS;
using BankBalance.BankInterfaces.Santander;
using BankBalanceWebService.Config;

namespace BankBalanceWebService
{
    public static class AccountManagerService
    {
        public static AccountManager AccountManager;

        static AccountManagerService()
        {
            AccountManager = new AccountManager();

            HalifaxConfig halifaxConfig;
            if (BankInterfaceConfigManager.TryGetConfig(out halifaxConfig))
            {
                AccountManager.TryAddInterface(config => new HalifaxInterface(config), halifaxConfig);
            }

            MAndSConfig mandsConfig;
            if (BankInterfaceConfigManager.TryGetConfig(out mandsConfig))
            {
                AccountManager.TryAddInterface(config => new MAndSInterface(config), mandsConfig);
            }

            SantanderConfig santanderConfig;
            if (BankInterfaceConfigManager.TryGetConfig(out santanderConfig))
            {
                AccountManager.TryAddInterface(config => new SantanderInterface(config), santanderConfig);
            }

            TsbConfig tsbConfig;
            if (BankInterfaceConfigManager.TryGetConfig(out tsbConfig))
            {
                AccountManager.TryAddInterface(config => new TsbInterface(config), tsbConfig);
            }

            SchedulePeriodicLoad(TimeSpan.FromMinutes(15));
        }

        private static void SchedulePeriodicLoad(TimeSpan timeBetweenLoads)
        {
            var periodicLoadingThread = new Thread(() =>
            {
                Thread.Sleep(timeBetweenLoads);
                AccountManager.Reload();
                SchedulePeriodicLoad(timeBetweenLoads);
            });
            periodicLoadingThread.Start();
        }
    }
}
