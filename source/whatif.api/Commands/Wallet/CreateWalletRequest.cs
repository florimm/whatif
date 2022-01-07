using Dapr.Client;
using MediatR;
using WhatIf.Api.Services;
using WhatIf.Api.States;

namespace WhatIf.Api.Commands.Wallet
{
    public record CreateWalletRequest(string Name) : IRequest<Guid>;

    public class CreateWalletHandler : IRequestHandler<CreateWalletRequest, Guid>
    {
        private readonly DaprClient daprClient;
        private readonly ICurrentUserService userService;
        public CreateWalletHandler(DaprClient daprClient, ICurrentUserService currentUserService)
        {
            this.daprClient = daprClient;
            userService = currentUserService;
        }
        
        public async Task<Guid> Handle(CreateWalletRequest request, CancellationToken cancellationToken)
        {
            string userId = userService.GetUserId();
            var userWallets = await daprClient.GetStateAsync<UserWallets>("statestore", $"{userId}-wallets");
            switch (userWallets)
            {
                case null:
                    userWallets = new UserWallets(userId, new List<States.Wallet> { new States.Wallet(Guid.NewGuid(), request.Name) });
                    break;
                default:
                    userWallets.Wallets.Add(new States.Wallet(Guid.NewGuid(), request.Name));
                    break;
            }
            await daprClient.SaveStateAsync("statestore", $"{userId}-wallets", userWallets);
            return userWallets.Wallets.Last().Id;
        }
    }
    
}