using System.Security.Claims;
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

        [HttpPost()]
        [AllowAnonymous]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUserRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpGet()]
        [Authorize]
        public ActionResult Data()
        {
            var identity = this.HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                return Ok(identity.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value);
            }
            return Ok("No data");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{userId}/wallets")]
        public async Task<ActionResult<GetAllWalletsForUserQueryResult>> Wallets([FromQuery] GetAllWalletsForUserQuery request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{userId}/wallets/{walletId}/investments")]
        public async Task<ActionResult<GetWalletInvestmentsQueryResult>> Investments([FromQuery] GetWalletInvestmentsQuery request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{userId}/wallets/refresh")]
        public async Task<ActionResult> Refresh([FromQuery] RefreshForUserAllWalletsRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }
    }
}