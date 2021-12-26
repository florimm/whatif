using Dapr;
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

        [HttpPost("price-change")]
        [Topic("pubsub", "price-change")]
        public async Task<ActionResult> Subscribe(PairPriceChanged data)
        {
            await _daprClient.SaveStateAsync<PairPriceChanged>("statestore", data.Pair.ToUpper(), data);
            _logger.LogInformation("PriceChangeController.Subscribe", data);
            return Ok();
        }

        [HttpGet("pair-price")]
        public async Task<ActionResult> GetPairPrice(string pair)
        {
            var result = await _daprClient.GetStateAsync<PairPriceChanged>("statestore", pair.ToUpper());
            return Ok(result);
        }
    }

}
