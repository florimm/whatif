using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using MediatR;
using WhatIf.Api.Actors;
using WhatIf.Api.States;

namespace WhatIf.Api.Commands.Wallet
{
    public record AddInvestmentRequest(Guid WalletId, string Symbol, double Amount) : IRequest<Investment>;

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