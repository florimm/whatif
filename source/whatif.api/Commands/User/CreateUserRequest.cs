using Dapr.Client;
using MediatR;
using WhatIf.Api.Utils;

namespace WhatIf.Api.Commands.User
{
    public record CreateUserRequest(string Email, string Password, string FirstName, string LastName) : IRequest<Guid>;

    public class CreateUserHandler : IRequestHandler<CreateUserRequest, Guid>
    {
        private readonly DaprClient daprClient;
        public CreateUserHandler(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }
        public async Task<Guid> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var userId = Guid.NewGuid();
            await daprClient.SaveStateAsync("statestore", request.Email, new WhatIf.Api.States.User(userId, request.Email, request.Password.Crypt(), request.FirstName, request.LastName));
            return userId;
        }
    }

}