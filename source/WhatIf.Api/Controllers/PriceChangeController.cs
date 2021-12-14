using Dapr.Actors.Client;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using WhatIf.Api.Actors;

namespace WhatIf.Api.Controllers
{
    [ApiController]
    public class PriceChangeController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<PriceChangeController> _logger;
        private readonly IActorProxyFactory _actorProxyFactory;

        public PriceChangeController(
            DaprClient daprClient,
            ILogger<PriceChangeController> logger,
            IActorProxyFactory actorProxyFactory)
        {
            _daprClient = daprClient;
            _logger = logger;
            _actorProxyFactory = actorProxyFactory;
        }

        [HttpPost("/price-change")]
        public async Task<IActionResult> Subscribe(PairPriceChanged data)
        {
            await Task.CompletedTask;
            // get wallets from state that have pairs same as data then update actors
            
            _logger.LogInformation("PriceChangeController.Subscribe", data);
            return Ok();
        }
    }

}
