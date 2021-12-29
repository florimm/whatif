using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using MediatR;
using WhatIf.Api.Actors;
using WhatIf.Api.States;

namespace WhatIf.Api.Commands.Wallet
{
    public record CreateWalletRequest(string UserId, string Name) : IRequest<Guid>;

    public class CreateWalletHandler : IRequestHandler<CreateWalletRequest, Guid>
    {
        private readonly DaprClient daprClient;
        public CreateWalletHandler(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }
        public async Task<Guid> Handle(CreateWalletRequest request, CancellationToken cancellationToken)
        {
            var userWallets = await daprClient.GetStateAsync<UserWallets>("statestore", $"{request.UserId}-wallets");
            switch (userWallets)
            {
                case null:
                    userWallets = new UserWallets(request.UserId, new List<WhatIf.Api.States.Wallet> { new WhatIf.Api.States.Wallet(Guid.NewGuid(), request.Name) });
                    break;
                default:
                    userWallets.Wallets.Add(new WhatIf.Api.States.Wallet(Guid.NewGuid(), request.Name));
                    break;
            }
            await daprClient.SaveStateAsync<UserWallets>("statestore", $"{request.UserId}-wallets", userWallets);
            return userWallets.Wallets.Last().Id;
        }
    }
    
}