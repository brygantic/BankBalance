# BankBalance

## Configure

### Locally
Go to the project directory (the directory containing BankBalance.sln) and run the following cmd command

    copy BankBalanceWebService\ConfigTemplate.xml ..\Settings\BankBalanceWebService\SecretWebConfig.config

You may need to create the necessary folders.
Open the file in a text editor and fill in any wanted config. Remove any you don't need.

### On Azure
* Deploy BankeBalanceWebService from VisualStudio
* FTP into the server hosting the web service
* Go to /
** There should be two folders: LogFiles and site
* Create a new folder called Settings
* Within that, create a new folder called BankBalanceWebService
* Copy your SecretWebConfig.config file to this folder


## Run
* Build and run BankBalanceWebService from Visual Studio
* Go to http://localhost:12345/swagger
* Enter one of your account numbers in Account > GET /account/{accountNumber}
* If the acount has been loaded, it will be returned to you
* If you get a 503, wait a minute or two for the web service to finish navigating the online banking sites
