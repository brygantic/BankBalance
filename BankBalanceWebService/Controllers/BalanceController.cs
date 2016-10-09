﻿using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using BankBalanceWebService.Dtos;

namespace BankBalanceWebService.Controllers
{
    [RoutePrefix("account")]
    public class BalanceController : ApiController
    {
        [HttpGet]
        [Route("{accountNumber}/balance")]
        [ResponseType(typeof(LightAccount))]
        public IHttpActionResult GetBalance(string accountNumber)
        {
            // Yes, accountNumber is not necessarily unique, but it's unique enough for now

            if (AccountManagerService.AccountManager.InitialLoadsComplete)
            {
                var account = AccountManagerService.AccountManager.GetAccount(accountNumber);
                return Ok(new LightAccount(account));
            }
            return Content(HttpStatusCode.ServiceUnavailable,
                "This service is not yet available. Try again in a few minutes.");
        }
    }
}
