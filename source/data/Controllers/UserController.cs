using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatIf.Api.Commands.User;
using WhatIf.Api.Commands.Wallet;
using WhatIf.Api.Queries.Wallet;

namespace WhatIf.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            this.mediator = mediator;
            _logger = logger;
        }

        [HttpPost(Name = "Create")]
        [AllowAnonymous]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUserRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPost(Name = "Login")]
        [AllowAnonymous]
        public async Task<ActionResult<Guid>> Login([FromBody] LoginRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpGet(Name = "{userId}/wallets")]
        public async Task<ActionResult<GetAllWalletsForUserQueryResult>> Wallets([FromQuery] GetAllWalletsForUserQuery request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpGet(Name = "{userId}/wallets/{walletId}/investments")]
        public async Task<ActionResult<GetWalletInvestmentsQueryResult>> Investments([FromQuery] GetWalletInvestmentsQuery request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpGet(Name = "{userId}/wallets/refresh")]
        public async Task<ActionResult> Refresh([FromQuery] RefreshForUserAllWalletsRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }
    }
}