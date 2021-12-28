using CSharpFunctionalExtensions;
using Dapr.Client;
using MediatR;
using WhatIf.Api.Services;
using WhatIf.Api.States;
using WhatIf.Api.Utils;

namespace WhatIf.Api.Commands.User
{
    public record LoginRequest(string Email, string Password) : IRequest<string>;
    
    public class LoginHandler : IRequestHandler<LoginRequest, string>
    {
        private readonly DaprClient daprClient;
        private readonly ITokenService tokenService;

        public LoginHandler(DaprClient daprClient, ITokenService tokenService)
        {
            this.daprClient = daprClient;
            this.tokenService = tokenService;
        }
        
        public async Task<string> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await daprClient.GetStateAsync<WhatIf.Api.States.User>("statestore", request.Email);
            if (user == null || user.Password != request.Password.Crypt())
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            var token = tokenService.BuildToken(user.Id, user.Email);
            return token;
        }
    }
}