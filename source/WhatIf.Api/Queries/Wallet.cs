using Dapr.Client;
using MediatR;
using WhatIf.Api.States;

namespace WhatIf.Api.Queries
{
    public record Wallet(Guid Id, string Name);
    public record GetAllWalletsForUserQueryResult(List<Wallet> Wallets);

    public class GetAllWalletsForUserQuery : IRequest<GetAllWalletsForUserQueryResult>
    {
        public string UserId { get; init; }
    }

    //handler
    public class GetAllWalletsForUserQueryHandler : IRequestHandler<GetAllWalletsForUserQuery, GetAllWalletsForUserQueryResult>
    {
        private readonly DaprClient daprClient;

        public GetAllWalletsForUserQueryHandler(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }

        public async Task<GetAllWalletsForUserQueryResult> Handle(GetAllWalletsForUserQuery request, CancellationToken cancellationToken)
        {
            var userWallets = await daprClient.GetStateAsync<UserWallets>("db", $"{request.Email}-wallets");
            return new GetAllWalletsForUserQueryResult(userWallets.Wallets.Select(t => new Wallet(t.Id, t.Name)).ToList());
        }
    }

    public record WalletInvestment(string Pair, double Amount);
    public record GetWalletInvestmentsQueryResult(List<WalletInvestment> Investments);

    public class GetWalletInvestmentsQuery : IRequest<GetWalletInvestmentsQueryResult>
    {
        public Guid WalletId { get; init; }
    }

    //handler
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