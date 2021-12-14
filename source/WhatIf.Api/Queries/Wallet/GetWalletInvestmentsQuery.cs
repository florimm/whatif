using Dapr.Client;
using MediatR;
using WhatIf.Api.States;

namespace WhatIf.Api.Queries.Wallet
{
    public record WalletInvestment(string Pair, double Amount);
    public record GetWalletInvestmentsQueryResult(List<WalletInvestment> Investments);

    public record GetWalletInvestmentsQuery(Guid WalletId) : IRequest<GetWalletInvestmentsQueryResult>;

    public class GetWalletInvestmentsQueryHandler : IRequestHandler<GetWalletInvestmentsQuery, GetWalletInvestmentsQueryResult>
    {
        private readonly DaprClient daprClient;

        public GetWalletInvestmentsQueryHandler(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }

        public async Task<GetWalletInvestmentsQueryResult> Handle(GetWalletInvestmentsQuery request, CancellationToken cancellationToken)
        {
            var walletInvestments = await daprClient.GetStateAsync<WalletInvestments>("db", request.WalletId.ToString());
            return new GetWalletInvestmentsQueryResult(walletInvestments.Investments.Select(t => new WalletInvestment(t.Pair, t.Amount)).ToList());
        }
    }

}