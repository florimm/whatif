using Dapr.Client;
using MediatR;
using WhatIf.Api.Services;
using WhatIf.Api.Utils;

namespace WhatIf.Api.Commands.User
{
    public record UpdateProfileRequest(string Email, string Password, string FirstName, string LastName) : IRequest<Unit>;

    public class UpdateProfileRequestHandler : IRequestHandler<UpdateProfileRequest, Unit>
    {
        private readonly DaprClient daprClient;
        private readonly ICurrentUserService userService;
        public UpdateProfileRequestHandler(DaprClient daprClient, ICurrentUserService currentUserService)
        {
            this.daprClient = daprClient;
            userService = currentUserService;
        }
        public async Task<Unit> Handle(UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            string email = userService.GetEmail();
            var user = await daprClient.GetStateAsync<States.User>("statestore", email);
            
            var data = user with {
                Name = request.FirstName,
                Surname = request.LastName,
                Password = request.Password.Encrypt(),
            };

            await daprClient.SaveStateAsync("statestore", email, data);

            return Unit.Value;
        }
    }

}