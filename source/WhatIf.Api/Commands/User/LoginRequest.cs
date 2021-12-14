using Dapr.Client;
using MediatR;
using WhatIf.Api.States;

namespace WhatIf.Api.Commands.User
{
    public record LoginRequest(string Email, string Password) : IRequest<Guid>;
    
    public class LoginHandler : IRequestHandler<LoginRequest, Guid>
    {
        private readonly DaprClient daprClient;
        public LoginHandler(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }
        
        public async Task<Guid> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await daprClient.GetStateAsync<WhatIf.Api.States.User>("db", request.Email);
            if (user == null || user.Password != request.Password)
            {
                return Guid.Empty;
            }
            return user.Id;
        }
    }
}