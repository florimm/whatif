using Dapr.Client;
using MediatR;
using WhatIf.Api.Services;
using WhatIf.Api.Utils;

namespace WhatIf.Api.Commands.User
{
    public record LoginRequest(string Email, string Password) : IRequest<LoginResponse>;

    public record LoginResponse(string Token, string UserName);
    
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly DaprClient daprClient;
        private readonly ITokenService tokenService;

        public LoginHandler(DaprClient daprClient, ITokenService tokenService)
        {
            this.daprClient = daprClient;
            this.tokenService = tokenService;
        }
        
        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await daprClient.GetStateAsync<States.User>("statestore", request.Email);
            if (user == null || user.Password != request.Password.Encrypt())
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            var token = tokenService.BuildToken(user.Id, user.Email);
            return new LoginResponse(token, user.Email);
        }
    }
}