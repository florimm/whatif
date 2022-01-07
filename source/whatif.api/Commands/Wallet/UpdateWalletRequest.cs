using Dapr.Client;
using MediatR;
using WhatIf.Api.Exceptions;
using WhatIf.Api.Services;
using WhatIf.Api.States;
using WhatIf.Api.Utils;

namespace WhatIf.Api.Commands.Wallet
{
    public record UpdateWalletRequest(Guid Id, string Name) : IRequest<Unit>;

    public class UpdateWalletHandler : IRequestHandler<UpdateWalletRequest, Unit>
    {
        private readonly DaprClient daprClient;
        private readonly ICurrentUserService userService;
        public UpdateWalletHandler(DaprClient daprClient, ICurrentUserService userService)
        {
            this.daprClient = daprClient;
            this.userService = userService;
        }
        public async Task<Unit> Handle(UpdateWalletRequest request, CancellationToken cancellationToken)
        {
            var userId = userService.GetUserId();
            var userWallets = await daprClient.GetStateAsync<UserWallets>("statestore", $"{userId}-wallets");
            var index = userWallets?.Wallets?.FindIndex(w => w.Id == request.Id);
            if (userWallets == null || index < 0)
            {
                throw new WalletNotFoundException();
            }
            
            var updatedUserWallets = userWallets with {
                                        Wallets = userWallets.Wallets.Replace(w => w.Id == request.Id, x => x with { Name = request.Name })
                                    };
            await daprClient.SaveStateAsync("statestore", $"{userId}-wallets", updatedUserWallets);
            return Unit.Value;
        }
    }
    
}