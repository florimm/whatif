using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using MediatR;
using WhatIf.Api.Actors;
using WhatIf.Api.Exceptions;
using WhatIf.Api.States;
using WhatIf.Api.Utils;

namespace WhatIf.Api.Commands.Wallet
{
    public record UpdateWalletRequest(Guid Id, string Email, string Name) : IRequest<Unit>;

    public class UpdateWalletHandler : IRequestHandler<UpdateWalletRequest, Unit>
    {
        private readonly DaprClient daprClient;
        public UpdateWalletHandler(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }
        public async Task<Unit> Handle(UpdateWalletRequest request, CancellationToken cancellationToken)
        {
            var userWallets = await daprClient.GetStateAsync<UserWallets>("statestore", $"{request.Email}-wallets");
            var index = userWallets?.Wallets?.FindIndex(w => w.Id == request.Id);
            if (userWallets == null || index < 0)
            {
                throw new WalletNotFoundException();
            }
            
            var updatedUserWallets = userWallets with {
                                        Wallets = userWallets.Wallets
                                            .Replace(w => w.Id == request.Id, x => x with { Name = request.Name })
                                    };
            await daprClient.SaveStateAsync<UserWallets>("statestore", $"{request.Email}-wallets", updatedUserWallets);
            return Unit.Value;
        }
    }
    
}