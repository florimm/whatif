using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WhatIf.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ExchangeInformationController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<ExchangeInformationController> _logger;
        private readonly DaprClient daprClient;

        public ExchangeInformationController(DaprClient daprClient, IMediator mediator, ILogger<ExchangeInformationController> logger)
        {
            this.daprClient = daprClient;
            this.mediator = mediator;
            _logger = logger;
        }       

        [HttpGet()]
        public ActionResult<List<string>> AllExhanges()
        {
            return Ok(
                new List<string> {
                "Binance"
            });
        }

        [HttpGet()]
        public ActionResult<List<string>> AllCryptoPairs()
        {
            //list of crypto pairs
            var cryptoPairs = new List<string> {
                "BTCUSDT",
                "ETHBTC",
                "ETHUSDT",
                "LTCBTC",
                "LTCUSDT",
                "BNBBTC",
                "BNBETH",
                "BNBUSDT",
                "NEOBTC",
                "NEOETH",
                "NEOBNB",
                "NEOUSDT",
                "QTUMBTC",
                "QTUMETH",
                "QTUMUSDT",
                "EOSBTC",
                "EOSETH",
                "EOSUSDT",
            };

            return Ok(cryptoPairs);
        }  
    }
}