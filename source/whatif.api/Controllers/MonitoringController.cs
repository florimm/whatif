using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WhatIf.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class MonitoringController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly DaprClient daprClient;

        public MonitoringController(DaprClient daprClient, IMediator mediator)
        {
            this.daprClient = daprClient;
            this.mediator = mediator;
        }
    }
}