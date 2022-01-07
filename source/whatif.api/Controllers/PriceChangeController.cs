using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatIf.Api.Actors;
using WhatIf.Api.Exceptions;

namespace WhatIf.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class PriceChangeController : ControllerBase
    {
        private readonly IActorProxyFactory actorProxyFactory;

        public PriceChangeController(IActorProxyFactory actorProxyFactory)
        {
            this.actorProxyFactory = actorProxyFactory;
        }

        [HttpGet("pair/{from}-{to}")]
        public async Task<ActionResult> GetPriceForPair([FromRoute]string from, [FromRoute]string to)
        {
            try
            {
                var proxy = actorProxyFactory.CreateActorProxy<IPairActor>(new ActorId($"{from.ToUpper()}{to.ToUpper()}"), nameof(PairActor));
                var result = await proxy.CurrentPrice("binance", from, to);
                return Ok(result);
            }
            catch(PairNotExistException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

}
