using Dapr;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using WhatIf.Api.Actors;
using WhatIf.Api.Exceptions;

namespace WhatIf.Api.Controllers
{
    [ApiController]
    public class PriceChangeController : ControllerBase
    {
        private readonly DaprClient daprClient;
        private readonly ILogger<PriceChangeController> logger;
        private readonly IActorProxyFactory actorProxyFactory;

        public PriceChangeController(
            DaprClient daprClient,
            ILogger<PriceChangeController> logger,
            IActorProxyFactory actorProxyFactory)
        {
            this.daprClient = daprClient;
            this.logger = logger;
            this.actorProxyFactory = actorProxyFactory;
        }

        [HttpPost("price-change")]
        [Topic("pubsub", "price-change")]
        public async Task<ActionResult> Subscribe(PairPriceChanged data)
        {
            System.Console.WriteLine($"Price Change: {data.From} {data.To} {data.Price}");
            await daprClient.SaveStateAsync<PairPriceChanged>("statestore", data.Pair, data);
            return Ok();
        }

        [HttpGet("pair-price/{pair}")]
        public async Task<ActionResult> GetPairPrice(string pair)
        {
            var result = await daprClient.GetStateAsync<PairPriceChanged>("statestore", pair.ToUpper());
            return Ok(result);
        }

        [HttpGet("pair/{from}-{to}")]
        public async Task<ActionResult> GetPriceForPair([FromRoute]string from, [FromRoute]string to)
        {
            try
            {
                var proxy = actorProxyFactory.CreateActorProxy<IPairActor>(new ActorId($"{from.ToUpper()}{to.ToUpper()}"), nameof(PairActor));
                var result = await proxy.CurrentPrice(from, to);
                return Ok(result);
            }
            catch(PairNotExistException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

}
