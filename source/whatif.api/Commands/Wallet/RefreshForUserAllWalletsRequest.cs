using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using MediatR;
using WhatIf.Api.Actors;
using WhatIf.Api.States;

namespace WhatIf.Api.Commands.Wallet
{
    public record RefreshForUserAllWalletsRequest(string Email) : IRequest;

    public class RefreshForUserAllWalletsHandler : IRequestHandler<RefreshForUserAllWalletsRequest>
    {
        private readonly IActorProxyFactory actorProxyFactory;
        private readonly DaprClient daprClient;
        public RefreshForUserAllWalletsHandler(DaprClient daprClient, IActorProxyFactory actorProxyFactory)
        {
            this.daprClient = daprClient;
            this.actorProxyFactory = actorProxyFactory;
        }
        public async Task<Unit> Handle(RefreshForUserAllWalletsRequest request, CancellationToken cancellationToken)
        {
            var userWallets = await daprClient.GetStateAsync<UserWallets>("statestore", $"{request.Email}-wallets");
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