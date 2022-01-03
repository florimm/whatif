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

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

    }
}