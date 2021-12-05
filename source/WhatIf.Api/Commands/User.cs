using Dapr.Client;
using MediatR;
using WhatIf.Api.States;

namespace WhatIf.Api.Commands
{
    public class CreateUserRequest : IRequest<Guid>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

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
            await daprClient.SaveStateAsync("db", request.Email, new User(userId, request.Email, request.Password, request.FirstName, request.LastName));
            return userId;
        }
    }

    public class LoginRequest : IRequest<Guid>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginHandler : IRequestHandler<LoginRequest, Guid>
    {
        private readonly DaprClient daprClient;
        public LoginHandler(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }
        
        public async Task<Guid> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await daprClient.GetStateAsync<User>("db", request.Email);
            if (user == null || user.Password != request.Password)
            {
                return Guid.Empty;
            }
            return user.Id;
        }
    }
}