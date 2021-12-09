using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using MediatR;
using WhatIf.Api.Actors;
using WhatIf.Api.States;

namespace WhatIf.Api.Commands
{
    public class CreateWalletRequest : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class CreateWalletHandler : IRequestHandler<CreateWalletRequest, Guid>
    {
        private readonly DaprClient daprClient;
        public CreateWalletHandler(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }
        public async Task<Guid> Handle(CreateWalletRequest request, CancellationToken cancellationToken)
        {
            var userWallets = await daprClient.GetStateAsync<UserWallets>("db", $"{request.Email}-wallets");
            switch (userWallets)
            {
                case null:
                    userWallets = new UserWallets(request.Email, new List<Wallet> { new Wallet(Guid.NewGuid(), request.Name) });
                    break;
                default:
                    userWallets.Wallets.Add(new Wallet(Guid.NewGuid(), request.Name));
                    break;
            }
            await daprClient.SaveStateAsync<UserWallets>("db", $"{request.Email}-wallets", userWallets);
            return userWallets.Wallets.Last().Id;
        }
    }
    
    public class RefreshForUserAllWalletsRequest : IRequest
    {
        public string Email { get; set; }
    }

    //handler for RefreshForUserAllWalletsRequest
    public class RefreshForUserAllWalletsHandler : IRequestHandler<RefreshForUserAllWalletsRequest>
    {
        private readonly ActorProxyFactory actorProxyFactory;
        private readonly DaprClient daprClient;
        public RefreshForUserAllWalletsHandler(DaprClient daprClient, ActorProxyFactory actorProxyFactory)
        {
            this.daprClient = daprClient;
            this.actorProxyFactory = actorProxyFactory;
        }
        public async Task<Unit> Handle(RefreshForUserAllWalletsRequest request, CancellationToken cancellationToken)
        {
            var userWallets = await daprClient.GetStateAsync<UserWallets>("db", $"{request.Email}-wallets");
            if (userWallets != null)
            {
                var investmentsPairs = new List<string>();
                foreach (var wallet in userWallets.Wallets)
                {
                    var investments = await daprClient.GetStateAsync<WalletInvestments>("db", wallet.Id.ToString());
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
    
    public class AddInvestmentRequest : IRequest<Investment>
    {
        public Guid WalletId { get; set; }
        public string Symbol { get; set; }
        public double Amount {get; set; }
    }

    public class AddInvestmentHandler : IRequestHandler<AddInvestmentRequest, Investment>
    {
        private readonly DaprClient daprClient;
        private readonly IActorProxyFactory actorProxyFactory;

        public AddInvestmentHandler(DaprClient daprClient, IActorProxyFactory actorProxyFactory)
        {
            this.daprClient = daprClient;
            this.actorProxyFactory = actorProxyFactory;
        }

        public async Task<Investment> Handle(AddInvestmentRequest request, CancellationToken cancellationToken)
        {
            var wallet = await daprClient.GetStateAsync<WalletInvestments>("db", request.WalletId.ToString());
            switch (wallet)
            {
                case null:
                    wallet = new WalletInvestments(request.WalletId, new List<Investment> { new Investment(request.Symbol, request.Amount) });
                    break;
                default:
                    wallet.Investments.Add(new Investment(request.Symbol, request.Amount));
                    break;
            }
            await daprClient.SaveStateAsync<WalletInvestments>("db", request.WalletId.ToString(), wallet);
            var proxy = actorProxyFactory.CreateActorProxy<IPairActor>(new ActorId($"{request.Symbol.ToUpper()}"), nameof(PairActor));
            await proxy.Monitor(new MonitorPairRequest(request.Symbol));
            return wallet.Investments.Last();
        }
    }
}