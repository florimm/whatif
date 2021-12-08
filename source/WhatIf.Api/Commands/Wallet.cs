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
    
    // add investment in wallet
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
            return wallet.Investments.Last();
        }
    }
}