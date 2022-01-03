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
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<UsersController> _logger;
        private readonly DaprClient daprClient;

        public UsersController(DaprClient daprClient, IMediator mediator, ILogger<UsersController> logger)
        {
            this.daprClient = daprClient;
            this.mediator = mediator;
            _logger = logger;
        }

        [HttpPost()]
        [AllowAnonymous]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUserRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{userId}/wallets")]
        public async Task<ActionResult<GetAllWalletsForUserQueryResult>> Wallets([FromRoute] GetAllWalletsForUserQuery request)
        {
            System.Console.WriteLine($"UserId ======> : {request.UserId}");
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("{userId}/wallets")]
        public async Task<ActionResult<Guid>> CreateWallet([FromBody] CreateWalletRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("{userId}/wallets/{walletId}")]
        public async Task<ActionResult> UpdateWallet([FromBody] UpdateWalletRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [HttpGet("{userId}/wallets/{walletId}/investments")]
        public async Task<ActionResult<GetWalletInvestmentsQueryResult>> Investments([FromRoute] GetWalletInvestmentsQuery request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("{userId}/wallets/{walletId}/investments")]
        public async Task<ActionResult> CreateInvestment([FromBody] CreateInvestmentRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [HttpPost("{userId}/wallets/refresh")]
        public async Task<ActionResult> Refresh([FromRoute] RefreshForUserAllWalletsRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }
    }
}