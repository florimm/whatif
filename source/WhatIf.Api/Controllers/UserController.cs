using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WhatIf.Api.Commands;

namespace WhatIf.Api.Controllers
{
    [ApiController]
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
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUserRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPost(Name = "Login")]
        public async Task<ActionResult<Guid>> Login([FromBody] LoginRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }
    }
}