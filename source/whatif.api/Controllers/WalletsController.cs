using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatIf.Api.Commands.User;
using WhatIf.Api.Commands.Wallet;
using WhatIf.Api.Models;
using WhatIf.Api.Queries.Wallet;

namespace WhatIf.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class WalletsController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<WalletsController> logger;
        private readonly DaprClient daprClient;

        public WalletsController(DaprClient daprClient, IMediator mediator, ILogger<WalletsController> logger)
        {
            this.daprClient = daprClient;
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpGet()]
        public async Task<ActionResult<GetAllWalletsForUserQueryResult>> Wallets([FromRoute] GetAllWalletsForUserQuery request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPost()]
        public async Task<ActionResult<Guid>> CreateWallet([FromBody] CreateWalletRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("{walletId}")]
        public async Task<ActionResult> UpdateWallet([FromBody] UpdateWalletRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [HttpGet("{walletId}/investments")]
        public async Task<ActionResult<GetWalletInvestmentsQueryResult>> Investments([FromRoute] GetWalletInvestmentsQuery request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("{walletId}/investments")]
        public async Task<ActionResult> CreateInvestment([FromBody] CreateInvestmentRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh([FromBody] RefreshForUserAllWalletsRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }
    }
}