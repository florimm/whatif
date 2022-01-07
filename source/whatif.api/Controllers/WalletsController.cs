using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatIf.Api.Commands.Wallet;
using WhatIf.Api.Queries.Wallet;

namespace WhatIf.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class WalletsController : ControllerBase
    {
        private readonly IMediator mediator;

        public WalletsController(IMediator mediator)
        {
            this.mediator = mediator;
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
        public async Task<ActionResult> CreateOrUpdateInvestment([FromBody] CreateOrUpdateInvestmentRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }
    }
}