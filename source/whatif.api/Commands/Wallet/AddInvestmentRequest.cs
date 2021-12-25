using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using MediatR;
using WhatIf.Api.Actors;
using WhatIf.Api.States;

namespace WhatIf.Api.Commands.Wallet
{
    public record AddInvestmentRequest(Guid WalletId, string Symbol, string From, string To, double Value, double Amount) : IRequest<Investment>;

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
            //patern matching
            var investment = new Investment(request.Symbol, request.From, request.To, request.Value, request.Amount);
            switch (wallet)
            {
                case null:
                    wallet = new WalletInvestments(request.WalletId, new List<Investment> { investment });
                    break;
                default:
                    wallet.Investments.Add(investment);
                    break;
            }
            await daprClient.SaveStateAsync<WalletInvestments>("db", request.WalletId.ToString(), wallet);
            var proxy = actorProxyFactory.CreateActorProxy<IPairActor>(new ActorId($"{request.Symbol.ToUpper()}"), nameof(PairActor));
            await proxy.Monitor(new MonitorPairRequest(request.Symbol));
            return wallet.Investments.Last();
        }
    }
}