using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WhatIf.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ExchangeInformationController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly DaprClient daprClient;

        public ExchangeInformationController(DaprClient daprClient, IMediator mediator)
        {
            this.daprClient = daprClient;
            this.mediator = mediator;
        }       

        [HttpGet()]
        public ActionResult<List<string>> AllExhanges()
        {
            return Ok(new List<string> { "Binance" });
        }
    }
}