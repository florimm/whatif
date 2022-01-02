using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using MediatR;
using WhatIf.Api.Actors;
using WhatIf.Api.Services;
using WhatIf.Api.States;

namespace WhatIf.Api.Commands.Wallet
{
    public record RefreshForUserAllWalletsRequest() : IRequest;

    public class RefreshForUserAllWalletsHandler : IRequestHandler<RefreshForUserAllWalletsRequest>
    {
        private readonly IActorProxyFactory actorProxyFactory;
        private readonly DaprClient daprClient;
        private readonly ICurrentUserService userService;
        public RefreshForUserAllWalletsHandler(DaprClient daprClient, IActorProxyFactory actorProxyFactory, ICurrentUserService currentUserService)
        {
            this.daprClient = daprClient;
            this.actorProxyFactory = actorProxyFactory;
            this.userService = currentUserService;
        }
        public async Task<Unit> Handle(RefreshForUserAllWalletsRequest request, CancellationToken cancellationToken)
        {
            var userId = userService.GetUserId();
            var userWallets = await daprClient.GetStateAsync<UserWallets>("statestore", $"{userId}-wallets");
            if (userWallets != null)
            {
                var investmentsPairs = new List<string>();
                foreach (var wallet in userWallets.Wallets)
                {
                    var investments = await daprClient.GetStateAsync<WalletInvestments>("statestore", wallet.Id.ToString());
                    investmentsPairs.AddRange(investments.Investments.Select(t => t.Pair.ToUpper()));
                }
                foreach (var pairs in investmentsPairs.Distinct())
                {
                    var proxy = actorProxyFactory.CreateActorProxy<IPairActor>(new ActorId($"{pairs}"), nameof(PairActor));
                    await proxy.Monitor(new MonitorPairRequest(pairs));
                }
            }
            return Unit.Value;
        }
    }
    
}