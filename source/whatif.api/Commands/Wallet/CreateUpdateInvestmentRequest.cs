using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using MediatR;
using WhatIf.Api.Actors;
using WhatIf.Api.States;

namespace WhatIf.Api.Commands.Wallet
{
    public record CreateOrUpdateInvestmentRequest(Guid WalletId, string From, string To, double Value, double Amount) : IRequest<Investment>;

    public class CreateOrUpdateInvestmentHandler : IRequestHandler<CreateOrUpdateInvestmentRequest, Investment>
    {
        private readonly DaprClient daprClient;
        private readonly IActorProxyFactory actorProxyFactory;

        public CreateOrUpdateInvestmentHandler(DaprClient daprClient, IActorProxyFactory actorProxyFactory)
        {
            this.daprClient = daprClient;
            this.actorProxyFactory = actorProxyFactory;
        }

        public async Task<Investment> Handle(CreateOrUpdateInvestmentRequest request, CancellationToken cancellationToken)
        {
            var wallet = await daprClient.GetStateAsync<WalletInvestments>("statestore", request.WalletId.ToString());
            var investment = new Investment(request.From, request.To, request.Value, request.Amount);
            if (wallet is null)
            {
                wallet = new WalletInvestments(request.WalletId, new List<Investment> { investment });
            }
            else
            {
                var index = wallet.Investments.FindIndex(t => t.From == request.From && t.To == request.To);
                if (index >= 0)
                {
                    wallet.Investments[index] = investment;
                }
                else
                {
                    wallet.Investments.Add(investment);
                }
            }
            await daprClient.SaveStateAsync("statestore", request.WalletId.ToString(), wallet);
            return investment;
        }
    }
}